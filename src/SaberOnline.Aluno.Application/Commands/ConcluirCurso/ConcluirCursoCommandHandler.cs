using MediatR;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.Comunications.AlunoCommands;
using SaberOnline.Core.SharedDto;


namespace SaberOnline.Aluno.Application.Commands.ConcluirCurso;
public class ConcluirCursoCommandHandler(IAlunoRepository alunoRepository,
    IMediatorHandler mediatorHandler) : IRequestHandler<ConcluirCursoCommand, bool>
{
    private readonly IAlunoRepository _alunoRepository = alunoRepository;
    private readonly IMediatorHandler _mediatorHandler = mediatorHandler;
    private Guid _raizAgregacao;

    public async Task<bool> Handle(ConcluirCursoCommand request, CancellationToken cancellationToken)
    {
        _raizAgregacao = request.RaizAgregacao;
        if (!ValidarRequisicao(request)) { return false; }
        if (!ObterAluno(request.AlunoId, out Domain.Entities.Aluno aluno)) { return false; }
        var matriculaCurso = aluno.ObterMatriculaCursoPeloId(request.MatriculaCursoId);

        if (!ValidarSeMatriculaCursoPodeSerConcluido(aluno, request.CursoDto)) { return false; }

        aluno.ConcluirCurso(request.MatriculaCursoId);

        await _alunoRepository.AtualizarAsync(aluno);
        return await _alunoRepository.UnitOfWork.Commit();
    }

    private bool ValidarRequisicao(ConcluirCursoCommand request)
    {
        request.DefinirValidacao(new ConcluirCursoCommandValidator().Validate(request));
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

    private bool ValidarSeMatriculaCursoPodeSerConcluido(Aluno.Domain.Entities.Aluno aluno, CursoDto cursoDto)
    {
        bool retorno = true;
        if (aluno.ObterQuantidadeAulasPendenteMatriculaCurso(cursoDto.Id) > 0)
        {
            retorno = false;
            _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Domain.Entities.Aluno), "Existem aulas pendentes para este curso")).GetAwaiter().GetResult();
        }

        int totalCursosAtivos = cursoDto.Aulas.Count(x => x.Ativo);
        int totalAulasMatricula = aluno.ObterQuantidadeAulasMatriculaCurso(cursoDto.Id);
        if (totalAulasMatricula < totalCursosAtivos)
        {
            retorno = false;
            _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Domain.Entities.Aluno), "Curso não pode ser concluído. Aulas pendentes.")).GetAwaiter().GetResult();
        }

        return retorno;
    }
}
