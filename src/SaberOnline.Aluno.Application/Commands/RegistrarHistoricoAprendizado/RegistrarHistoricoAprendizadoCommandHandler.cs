using MediatR;
using Plataforma.Educacao.Aluno.Application.Commands.RegistrarHistoricoAprendizado;

using SaberOnline.Aluno.Domain.Entities;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Aluno.Domain.ValueObjects;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoEvents;
using SaberOnline.Core.Messages.Comunications.AlunoCommands;
using SaberOnline.Core.SharedDto;


namespace SaberOnline.Aluno.Application.Commands.RegistrarHistoricoAprendizado;
public class RegistrarHistoricoAprendizadoCommandHandler(IAlunoRepository alunoRepository, 
    IMediatorHandler mediatorHandler) : IRequestHandler<RegistrarHistoricoAprendizadoCommand, bool>
{
    private readonly IAlunoRepository _alunoRepository = alunoRepository;
    private readonly IMediatorHandler _mediatorHandler = mediatorHandler;
    private Guid _raizAgregacao;

    public async Task<bool> Handle(RegistrarHistoricoAprendizadoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _raizAgregacao = request.RaizAgregacao;
            if (!ValidarRequisicao(request)) { return false; }
            if (!ObterAluno(request.AlunoId, out Domain.Entities.Aluno aluno)) { return false; }
            if (!ObterAulaCurso(request.CursoDto, request.AulaId, aluno, out AulaDto aulaDto)) { return false; }

            MatriculaCurso matriculaCurso = aluno.ObterMatriculaCursoPeloId(request.MatriculaCursoId);
            HistoricoAprendizado historicoAntigo = aluno.ObterHistoricoAprendizado(request.MatriculaCursoId, request.AulaId);

            aluno.RegistrarHistoricoAprendizado(request.MatriculaCursoId, request.AulaId, aulaDto.Descricao, request.DataTermino);

            HistoricoAprendizado historicoAtual = aluno.ObterHistoricoAprendizado(request.MatriculaCursoId, request.AulaId);

            await _alunoRepository.AtualizarEstadoHistoricoAprendizadoAsync(historicoAntigo, historicoAtual);
            return await _alunoRepository.UnitOfWork.Commit();
        }
        catch (Exception ex)
        {
            string mensagem = $"Erro registrando histórico de Aprendizado. Exception: {ex}";
            await _mediatorHandler.PublicarEvento(new RegistrarProblemaHistoricoAprendizadoEvent(request.AlunoId, 
                request.MatriculaCursoId, 
                request.AulaId, 
                request.DataTermino, 
                mensagem));
            throw;
        }
    }

    private bool ValidarRequisicao(RegistrarHistoricoAprendizadoCommand request)
    {
        request.DefinirValidacao(new RegistrarHistoricoAprendizadoCommandValidator().Validate(request));

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

    private bool ObterAulaCurso(CursoDto cursoDto, Guid aulaId, Domain.Entities.Aluno aluno, out AulaDto aulaDto)
    {

        aulaDto = cursoDto?.Aulas?.FirstOrDefault(x => x.Id == aulaId) ?? new();

        if (!aulaDto.Ativo)
        {
            _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Domain.Entities.Aluno), "Aula informada está inativa")).GetAwaiter().GetResult();
            return false;
        }

        return true;
    }
}
