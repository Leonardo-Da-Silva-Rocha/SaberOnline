using MediatR;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoCommands;

namespace SaberOnline.Aluno.Application.Commands.CadastrarAluno
{
    public class CadastrarAlunoCommandHandler : IRequestHandler<CadastrarAlunoCommand, bool>
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IMediatorHandler _mediatorHandler;
        private Guid _raizAgregacao;

        public CadastrarAlunoCommandHandler(IAlunoRepository alunoRepository, IMediatorHandler mediatorHandler)
        {
            _alunoRepository = alunoRepository;
            _mediatorHandler = mediatorHandler;
        }

        public async Task<bool> Handle(CadastrarAlunoCommand request, CancellationToken cancellationToken)
        {
            _raizAgregacao = request.RaizAgregacao;
            if (!ValidarRequisicao(request)) { return false; }

            var aluno = new Domain.Entities.Aluno(request.Nome, request.Email, request.DataNascimento);
            aluno.IdentificarCodigoUsuarioNoSistema(request.AlunoId);
            await _alunoRepository.AdicionarAsync(aluno);

            return await _alunoRepository.UnitOfWork.Commit();
        }

        private bool ValidarRequisicao(CadastrarAlunoCommand request)
        {
            request.DefinirValidacao(new CadastrarAlunoCommandValidator().Validate(request));
            if (!request.EhValido())
            {
                foreach (var erro in request.Erros)
                {
                    _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Domain.Entities.Aluno), erro)).GetAwaiter().GetResult();
                }
                return false;
            }

            return true;
        }
    }
}
