using FluentAssertions;
using Moq;
using SaberOnline.Aluno.Application.Commands.SolicitarCertificado;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Agregrates;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoCommands;

namespace SaberOnline.Aluno.Tests.Applications.Commands;

public class SolicitarCertificadoCommandHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
    private readonly SolicitarCertificadoCommandHandler _handler;

    public SolicitarCertificadoCommandHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _mediatorHandlerMock = new Mock<IMediatorHandler>();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _alunoRepositoryMock.Setup(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

        _handler = new SolicitarCertificadoCommandHandler(
            _alunoRepositoryMock.Object,
            _mediatorHandlerMock.Object
        );
    }

    [Fact]
    public async Task Deve_retornar_false_quando_requisicao_invalida()
    {
        // Arrange
        var command = new SolicitarCertificadoCommand(Guid.Empty, Guid.Empty, string.Empty);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeFalse();
        _mediatorHandlerMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Deve_retornar_false_quando_aluno_nao_encontrado()
    {
        // Arrange
        var command = CriarComandoValido();

        _alunoRepositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Domain.Entities.Aluno)null);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeFalse();
        _mediatorHandlerMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Deve_solicitar_certificado_com_sucesso()
    {
        // Arrange
        var aluno = CriarAlunoComCursoEAulasConcluidas();
        var matricula = aluno.MatriculasCursos.First();

        aluno.ConcluirCurso(matricula.Id);

        _alunoRepositoryMock.Setup(r => r.ObterPorIdAsync(aluno.Id)).ReturnsAsync(aluno);

        var command = new SolicitarCertificadoCommand(aluno.Id, matricula.Id, "caminho/do/certificado.pdf");

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeTrue();
    }

    #region Helpers

    private static SolicitarCertificadoCommand CriarComandoValido()
    {
        return new SolicitarCertificadoCommand(Guid.NewGuid(), Guid.NewGuid(), "caminho/do/certificado.pdf");
    }


    private static Domain.Entities.Aluno CriarAlunoComCursoEAulasConcluidas()
    {
        var aluno = new Domain.Entities.Aluno("Aluno Teste", "teste@email.com", new DateTime(1990, 1, 1));
        var cursoId = Guid.NewGuid();
        var aulaId1 = Guid.NewGuid();
        var aulaId2 = Guid.NewGuid();

        aluno.MatricularEmCurso(cursoId, "Curso Teste", 500);
        var matricula = aluno.MatriculasCursos.First();
        aluno.AtualizarPagamentoMatricula(matricula.Id);

        matricula.RegistrarHistoricoAprendizado(aulaId1, "Aula 1", DateTime.Now);
        matricula.RegistrarHistoricoAprendizado(aulaId2, "Aula 2", DateTime.Now);

        return aluno;
    }
    #endregion
}
