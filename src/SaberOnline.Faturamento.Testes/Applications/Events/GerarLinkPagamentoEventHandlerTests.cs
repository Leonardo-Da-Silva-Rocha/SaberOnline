using Moq;
using SaberOnline.Application.Events.GerarLinkPagamento;
using SaberOnline.Core.Agregrates;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.FaturamentoEvents;
using SaberOnline.Faturamento.Domain.Entities;
using SaberOnline.Faturamento.Domain.Interfaces;

namespace SaberOnline.Faturamento.Tests.Applications.Events;
public class GerarLinkPagamentoEventHandlerTests
{
    private readonly Mock<IFaturamentoRepository> _repoMock = new();
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly GerarLinkPagamentoEventHandler _handler;

    public GerarLinkPagamentoEventHandlerTests()
    {
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _repoMock.Setup(r => r.UnitOfWork).Returns(uowMock.Object);

        _handler = new GerarLinkPagamentoEventHandler(_repoMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task Deve_gerar_link_pagamento_com_evento_valido()
    {
        var evento = new GerarLinkPagamentoEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 500);

        await _handler.Handle(evento, CancellationToken.None);

        _repoMock.Verify(r => r.AdicionarAsync(It.IsAny<Pagamento>()), Times.Once);
        _repoMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task Nao_deve_gerar_link_quando_evento_invalido()
    {
        var evento = new GerarLinkPagamentoEvent(Guid.Empty, Guid.Empty, Guid.Empty, 0);

        await _handler.Handle(evento, CancellationToken.None);

        _repoMock.Verify(r => r.AdicionarAsync(It.IsAny<Pagamento>()), Times.Never);
        _repoMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
        _mediatorMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
    }
}