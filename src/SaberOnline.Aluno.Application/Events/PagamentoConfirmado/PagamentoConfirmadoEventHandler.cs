using MediatR;
using SaberOnline.Aluno.Application.AtualizarPagamento;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.FaturamentoEvents;



namespace SaberOnline.Aluno.Application.PagamentoConfirmado;
public class PagamentoConfirmadoEventHandler(IAlunoRepository alunoRepository,
    IMediatorHandler mediatorHandler) : INotificationHandler<PagamentoConfirmadoEvent>
{
    private readonly IAlunoRepository _alunoRepository = alunoRepository;
    private readonly IMediatorHandler _mediatorHandler = mediatorHandler;
    private Guid _raizAgregacao;

    public async Task Handle(PagamentoConfirmadoEvent notification, CancellationToken cancellationToken)
    {
        _raizAgregacao = notification.RaizAgregacao;
        if (!ValidarRequisicao(notification)) { return; }
       
        if (!ObterAluno(notification.AlunoId, out Domain.Entities.Aluno aluno)) { return; }

        var matricula = aluno.ObterMatriculaPorCursoId(notification.CursoId);
        aluno.AtualizarPagamentoMatricula(matricula.Id);

        await _alunoRepository.AtualizarAsync(aluno);
        await _alunoRepository.UnitOfWork.Commit();
    }

    private bool ValidarRequisicao(PagamentoConfirmadoEvent notification)
    {
        notification.DefinirValidacao(new PagamentoConfirmadoEventValidator().Validate(notification));
        if (!notification.EhValido())
        {
            foreach (var erro in notification.Erros)
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