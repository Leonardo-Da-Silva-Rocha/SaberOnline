
using FluentAssertions;
using Moq;
using SaberOnline.Aluno.Application.Commands.MatricularAluno;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Agregrates;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoCommands;

namespace SaberOnline.Aluno.Tests.Applications.Commands;
public class MatricularAlunoCommandHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<IMediatorHandler> _mediatorHandlerMock;

    private readonly MatricularAlunoCommandHandler _handler;

    public MatricularAlunoCommandHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _mediatorHandlerMock = new Mock<IMediatorHandler>();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _alunoRepositoryMock.Setup(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

        _handler = new MatricularAlunoCommandHandler(
            _alunoRepositoryMock.Object,
            _mediatorHandlerMock.Object
        );
    }

    [Fact]
    public async Task Handle_Deve_Matricular_Aluno_Quando_Tudo_Valido()
    {
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var aluno = new Domain.Entities.Aluno("Jairo Azevedo", "jairo.azevedo@email.com", new DateTime(1973, 06, 25));

        _alunoRepositoryMock.Setup(r => r.ObterPorIdAsync(alunoId)).ReturnsAsync(aluno);

        var command = new MatricularAlunoCommand(alunoId, cursoId, true, "Curso de desenvolvimento de sistemas com Angular", 1800.00m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        
    }

    [Fact]
    public async Task Handle_Deve_Publicar_Notificacao_Se_Curso_Indisponivel()
    {
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();

        var command = new MatricularAlunoCommand(alunoId, cursoId, false, "Curso de desenvolvimento de sistemas com Angular", 1800.00m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _mediatorHandlerMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Deve_Publicar_Notificacao_Se_Aluno_Nao_Encontrado()
    {
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();

        _alunoRepositoryMock.Setup(r => r.ObterPorIdAsync(alunoId)).ReturnsAsync((Domain.Entities.Aluno)null);

        var command = new MatricularAlunoCommand(alunoId, cursoId, true, "Curso de desenvolvimento de sistemas com Angular", 1800.00m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _mediatorHandlerMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Deve_Publicar_Notificacao_Se_Command_Invalido()
    {
        var command = new MatricularAlunoCommand(Guid.Empty, Guid.Empty, false, string.Empty, 0.00m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _mediatorHandlerMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_Deve_retornar_false_se_commit_falhar()
    {
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var aluno = new Domain.Entities.Aluno("Jairo", "jairo.souza@email.com", new DateTime(1973, 06, 25));

        _alunoRepositoryMock.Setup(r => r.ObterPorIdAsync(alunoId)).ReturnsAsync(aluno);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);
        _alunoRepositoryMock.Setup(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

        var command = new MatricularAlunoCommand(alunoId, cursoId, false, "Curso de SOLID", 800.00m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
