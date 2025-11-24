using MediatR;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.FaturamentoEvents;


namespace SaberOnline.Aluno.Application.PagamentoRecusado;
public class PagamentoRecusadoEventHandler(IMediatorHandler mediatorHandler) : INotificationHandler<PagamentoRecusadoEvent>
{
    private readonly IMediatorHandler _mediatorHandler = mediatorHandler;
    private Guid _raizAgregacao;

    public async Task Handle(PagamentoRecusadoEvent notification, CancellationToken cancellationToken)
    {
        _raizAgregacao = notification.RaizAgregacao;
        if (!ValidarRequisicao(notification)) { return; }
    }

    private bool ValidarRequisicao(PagamentoRecusadoEvent notification)
    {
        notification.DefinirValidacao(new PagamentoRecusadoEventValidator().Validate(notification));
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
}