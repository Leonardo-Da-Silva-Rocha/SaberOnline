using MediatR;
using SaberOnline.Faturamento.Domain.Interfaces;
using SaberOnline.Core.Messages.FaturamentoEvents;
using SaberOnline.Core.Messages;
using SaberOnline.Faturamento.Domain.Entities;



namespace SaberOnline.Application.Events.GerarLinkPagamento;
public class GerarLinkPagamentoEventHandler(IFaturamentoRepository faturamentoRepository,
    IMediatorHandler mediatorHandler) : INotificationHandler<GerarLinkPagamentoEvent>
{
    private readonly IFaturamentoRepository _faturamentoRepository = faturamentoRepository;
    private readonly IMediatorHandler _mediatorHandler = mediatorHandler;
    private Guid _raizAgregacao;

    public async Task Handle(GerarLinkPagamentoEvent request, CancellationToken cancellationToken)
    {
        _raizAgregacao = request.RaizAgregacao;
        if (!ValidarRequisicao(request)) { return; }

        var pagamento = new Pagamento(request.MatriculaCursoId, request.Valor, request.DataHora.AddDays(7).Date);
        await _faturamentoRepository.AdicionarAsync(pagamento);

        await _faturamentoRepository.UnitOfWork.Commit();
    }

    private bool ValidarRequisicao(GerarLinkPagamentoEvent notification)
    {
        notification.DefinirValidacao(new GerarLinkPagamentoEventValidator().Validate(notification));
        if (!notification.EhValido())
        {
            foreach (var erro in notification.Erros)
            {
                _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Pagamento), erro)).GetAwaiter().GetResult();
            }
            return false;
        }

        return true;
    }
}
