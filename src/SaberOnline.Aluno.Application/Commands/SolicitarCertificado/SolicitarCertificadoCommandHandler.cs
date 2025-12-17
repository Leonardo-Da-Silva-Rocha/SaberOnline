using MediatR;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoCommands;

namespace SaberOnline.Aluno.Application.Commands.SolicitarCertificado;
public class SolicitarCertificadoCommandHandler(IAlunoRepository alunoRepository, IMediatorHandler mediatorHandler) : IRequestHandler<SolicitarCertificadoCommand, bool>
{
    private readonly IAlunoRepository _alunoRepository = alunoRepository;
    private readonly IMediatorHandler _mediatorHandler = mediatorHandler;
    private Guid _raizAgregacao;

    public async Task<bool> Handle(SolicitarCertificadoCommand request, CancellationToken cancellationToken)
    {
        _raizAgregacao = request.RaizAgregacao;
        if (!ValidarRequisicao(request)) { return false; }
        if (!ObterAluno(request.AlunoId, out SaberOnline.Aluno.Domain.Entities.Aluno aluno)) { return false; }

        aluno.RequisitarCertificadoConclusao(request.MatriculaCursoId, request.PathCertificado);
        var certificado = aluno.ObterMatriculaCursoPeloId(request.MatriculaCursoId).Certificado;

        await _alunoRepository.AdicionarCertificadoMatriculaCursoAsync(certificado);
        return await _alunoRepository.UnitOfWork.Commit();
    }

    private bool ValidarRequisicao(SolicitarCertificadoCommand request)
    {
        request.DefinirValidacao(new SolicitarCertificadoCommandValidator().Validate(request));

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

    private bool ObterAluno(Guid alunoId, out SaberOnline.Aluno.Domain.Entities.Aluno aluno)
    {
        aluno = _alunoRepository.ObterPorIdAsync(alunoId).Result;
        if (aluno == null)
        {
            _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Domain.Entities.Aluno), "Aluno não encontrado.")).GetAwaiter().GetResult();
            return false;
        }

        return true;
    }
}
