using Moq;
using SaberOnline.Aluno.Application.PagamentoConfirmado;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.FaturamentoEvents;


namespace SaberOnline.Aluno.Tests.Applications.Events;
public class PagamentoConfirmadoEventHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepoMock = new();
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly PagamentoConfirmadoEventHandler _handler;

    public PagamentoConfirmadoEventHandlerTests()
    {
        _handler = new PagamentoConfirmadoEventHandler(
            _alunoRepoMock.Object,
            _mediatorMock.Object
        );
    }

    [Fact]
    public async Task Deve_atualizar_pagamento_quando_evento_valido()
    {
        var aluno = new Domain.Entities.Aluno("Teste", "teste@email.com", new DateTime(1990, 1, 1));
        var cursoId = Guid.NewGuid();
        aluno.MatricularEmCurso(cursoId, "Curso Teste", 500);
        var matricula = aluno.MatriculasCursos.First();

        _alunoRepoMock.Setup(r => r.ObterPorIdAsync(aluno.Id)).ReturnsAsync(aluno);
        _alunoRepoMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(true);

        var evento = new PagamentoConfirmadoEvent(matricula.Id, aluno.Id, cursoId, true);

        await _handler.Handle(evento, CancellationToken.None);

        _alunoRepoMock.Verify(r => r.AtualizarAsync(It.IsAny<Domain.Entities.Aluno>()), Times.Once);
        _alunoRepoMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task Deve_publicar_notificacao_quando_evento_invalido()
    {
        var evento = new PagamentoConfirmadoEvent(Guid.Empty, Guid.Empty, Guid.Empty, false);

        await _handler.Handle(evento, CancellationToken.None);

        _mediatorMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Deve_publicar_notificacao_quando_aluno_nao_encontrado()
    {
        var evento = new PagamentoConfirmadoEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false);

        _alunoRepoMock.Setup(r => r.ObterPorIdAsync(evento.AlunoId)).ReturnsAsync((Domain.Entities.Aluno?)null);

        await _handler.Handle(evento, CancellationToken.None);

        _mediatorMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Deve_publicar_notificacao_quando_curso_indisponivel()
    {
        var evento = new PagamentoConfirmadoEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false);

        await _handler.Handle(evento, CancellationToken.None);

        _mediatorMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
    }
}