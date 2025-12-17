using FluentAssertions;
using Moq;
using SaberOnline.Aluno.Application.Commands.CadastrarAluno;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Agregrates;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoCommands;


namespace SaberOnline.Aluno.Tests.Applications.Commands;
public class CadastrarAlunoCommandHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
    private readonly CadastrarAlunoCommandHandler _handler;

    public CadastrarAlunoCommandHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _mediatorHandlerMock = new Mock<IMediatorHandler>();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _alunoRepositoryMock.Setup(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

        _handler = new CadastrarAlunoCommandHandler(
            _alunoRepositoryMock.Object,
            _mediatorHandlerMock.Object
        );
    }

    [Fact]
    public async Task Deve_retornar_false_quando_requisicao_invalida()
    {
        // Arrange
        var command = new CadastrarAlunoCommand(Guid.Empty, "", "emailinvalido", DateTime.Today.AddDays(1));

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeFalse();
        _mediatorHandlerMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Deve_cadastrar_aluno_com_sucesso()
    {
        // Arrange
        var command = new CadastrarAlunoCommand(Guid.NewGuid(), "Aluno Teste", "teste@email.com", new DateTime(1995, 5, 15));

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeTrue();
        _alunoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.Aluno>()), Times.Once);
    }

    [Fact]
    public async Task Deve_retornar_false_quando_commit_falhar()
    {
        var command = new CadastrarAlunoCommand(Guid.NewGuid(), "Aluno Teste", "teste@email.com", new DateTime(1995, 5, 15));

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);
        _alunoRepositoryMock.Setup(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

        var handler = new CadastrarAlunoCommandHandler(_alunoRepositoryMock.Object, _mediatorHandlerMock.Object);

        var resultado = await handler.Handle(command, CancellationToken.None);

        resultado.Should().BeFalse();
    }
}