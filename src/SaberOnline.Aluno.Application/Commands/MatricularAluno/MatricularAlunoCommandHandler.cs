
using MediatR;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoCommands;
using SaberOnline.Core.Messages.FaturamentoEvents;



namespace SaberOnline.Aluno.Application.Commands.MatricularAluno;
public class MatricularAlunoCommandHandler(IAlunoRepository alunoRepository,
    IMediatorHandler mediatorHandler) : IRequestHandler<MatricularAlunoCommand, bool>
{
    private readonly IAlunoRepository _alunoRepository = alunoRepository;
    private readonly IMediatorHandler _mediatorHandler = mediatorHandler;
    private Guid _raizAgregacao;

    public async Task<bool> Handle(MatricularAlunoCommand request, CancellationToken cancellationToken)
    {
        _raizAgregacao = request.RaizAgregacao;
        if (!ValidarRequisicao(request)) { return false; }
        if (!ObterAluno(request.AlunoId, out Domain.Entities.Aluno aluno)) { return false; }

        aluno.MatricularEmCurso(request.CursoId, request.NomeCurso, request.ValorCurso);
        var matricula = aluno.ObterMatriculaPorCursoId(request.CursoId);
        await _alunoRepository.AdicionarMatriculaCursoAsync(matricula);
        await _alunoRepository.UnitOfWork.Commit();

        await _mediatorHandler.PublicarEvento(new GerarLinkPagamentoEvent(matricula.Id, request.AlunoId, request.CursoId, request.ValorCurso));

        return true;
    }

    private bool ValidarRequisicao(MatricularAlunoCommand request)
    {
        request.DefinirValidacao(new MatricularAlunoCommandValidator().Validate(request));
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

    private bool ObterAluno(Guid alunoId, out Domain.Entities.Aluno aluno)
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
