
using FluentAssertions;
using Moq;

using SaberOnline.Aluno.Application.Commands.ConcluirCurso;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Agregrates;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.Comunications.AlunoCommands;
using SaberOnline.Core.SharedDto;

namespace SaberOnline.Aluno.Tests.Applications.Commands;
public class ConcluirCursoCommandHandlerTests
{

    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
    private readonly ConcluirCursoCommandHandler _handler;

    public ConcluirCursoCommandHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _mediatorHandlerMock = new Mock<IMediatorHandler>();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _alunoRepositoryMock.Setup(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

        _handler = new ConcluirCursoCommandHandler(
            _alunoRepositoryMock.Object,
            _mediatorHandlerMock.Object
        );
    }

    [Fact]
    public async Task Deve_retornar_false_quando_requisicao_invalida()
    {
        // Arrange
        var command = new ConcluirCursoCommand(Guid.Empty, Guid.Empty, null);

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
        var command = new ConcluirCursoCommand(Guid.NewGuid(), Guid.NewGuid(), new CursoDto());

        _alunoRepositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Domain.Entities.Aluno)null);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeFalse();
        _mediatorHandlerMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Deve_concluir_curso_com_sucesso()
    {
        // Arrange
        var cursoId = Guid.NewGuid();

        var aluno = CriarAlunoComMatriculaECurtsoPorConcluir(cursoId);
        _alunoRepositoryMock.Setup(r => r.ObterPorIdAsync(aluno.Id)).ReturnsAsync(aluno);

        var curso = new CursoDto
        {
            Id = cursoId,
            Nome = "Curso Teste",
            Valor = 500,
            CursoDisponivel = true,
            Aulas = new List<AulaDto> { new() { Id = Guid.NewGuid(), Descricao = "Aula de refatoração 01", Ativo = true } }
        };

        var command = new ConcluirCursoCommand(aluno.Id, aluno.MatriculasCursos.Last().Id, curso);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeTrue();
        _alunoRepositoryMock.Verify(r => r.AtualizarAsync(aluno), Times.Once);
    }

    [Fact]
    public async Task Deve_retornar_false_quando_nao_existem_aulas_ativas()
    {
        var cursoId = Guid.NewGuid();
        var curso = new CursoDto
        {
            Id = cursoId,
            Nome = "Curso Teste",
            Valor = 500,
            CursoDisponivel = true,
            Aulas = new List<AulaDto> { new() { Id = Guid.NewGuid(), Descricao = "Aula de refatoração 01", Ativo = false } }
        };


        var aluno = CriarAlunoComMatriculaECurtsoConcluido(cursoId);
        _alunoRepositoryMock.Setup(r => r.ObterPorIdAsync(aluno.Id)).ReturnsAsync(aluno);

        var command = new ConcluirCursoCommand(aluno.Id, aluno.MatriculasCursos.Last().Id, curso);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().BeFalse();
        _mediatorHandlerMock.Verify(m => m.PublicarNotificacaoDominio(It.IsAny<DomainNotificacaoRaiz>()), Times.AtLeastOnce);
    }

    #region Helpers


    private static Domain.Entities.Aluno CriarAlunoValido()
    {
        var aluno = new Domain.Entities.Aluno("Aluno Teste", "teste@email.com", new DateTime(1990, 1, 1));
        aluno.MatricularEmCurso(Guid.NewGuid(), "Curso Teste", 500);
        return aluno;
    }

    private static Domain.Entities.Aluno CriarAlunoComMatriculaECurtsoConcluido(Guid cursoId)
    {
        Guid aulaId1 = Guid.NewGuid();

        var aluno = new Domain.Entities.Aluno("Aluno Teste", "teste@email.com", new DateTime(1995, 1, 1));
        aluno.MatricularEmCurso(cursoId, "Curso Teste", 100);

        Guid matriculaCursoId1 = aluno.MatriculasCursos.First().Id;

        aluno.AtualizarPagamentoMatricula(matriculaCursoId1);
        aluno.RegistrarHistoricoAprendizado(matriculaCursoId1, aulaId1, "Aula Teste 1", null);
        aluno.RegistrarHistoricoAprendizado(matriculaCursoId1, aulaId1, "Aula Teste 1", DateTime.Now.Date);
        aluno.ConcluirCurso(matriculaCursoId1);

        aluno.RequisitarCertificadoConclusao(matriculaCursoId1, "/caminho/certificado1.pdf");

        return aluno;
    }

    private static Domain.Entities.Aluno CriarAlunoComMatriculaECurtsoPorConcluir(Guid cursoId)
    {
        Guid aulaId1 = Guid.NewGuid();
        var aluno = new Domain.Entities.Aluno("Aluno Teste", "teste@email.com", new DateTime(1995, 1, 1));
        aluno.MatricularEmCurso(cursoId, "Curso Teste", 100);
        Guid matriculaCursoId1 = aluno.MatriculasCursos.First().Id;
        aluno.AtualizarPagamentoMatricula(matriculaCursoId1);
        aluno.RegistrarHistoricoAprendizado(matriculaCursoId1, aulaId1, "Aula Teste 1", DateTime.Now.Date);
        return aluno;
    }

    #endregion
}