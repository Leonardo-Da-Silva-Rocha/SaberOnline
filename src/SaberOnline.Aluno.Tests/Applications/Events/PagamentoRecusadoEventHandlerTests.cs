using Moq;
using SaberOnline.Aluno.Application.PagamentoRecusado;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.FaturamentoEvents;

namespace SaberOnline.Aluno.Tests.Applications.Events;
public class PagamentoRecusadoEventHandlerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock;
    private readonly PagamentoRecusadoEventHandler _handler;

    public PagamentoRecusadoEventHandlerTests()
    {
        _mediatorMock = new Mock<IMediatorHandler>();
        _handler = new PagamentoRecusadoEventHandler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Deve_publicar_notificacao_quando_evento_invalido()
    {
        var evento = new PagamentoRecusadoEvent(Guid.Empty, Guid.Empty, Guid.Empty, string.Empty);

        await _handler.Handle(evento, CancellationToken.None);

        _mediatorMock.Verify(m => m.PublicarNotificacaoDominio(
            It.Is<DomainNotificacaoRaiz>(n => n.RaizAgregacao == evento.RaizAgregacao)), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Nao_deve_publicar_nada_quando_evento_valido()
    {
        var evento = new PagamentoRecusadoEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Falha no pagamento");

        await _handler.Handle(evento, CancellationToken.None);

        _mediatorMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.Never);
    }
}