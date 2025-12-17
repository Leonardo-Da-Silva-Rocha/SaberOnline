using FluentAssertions;
using Moq;
using SaberOnline.Conteudo.Application.Services;
using SaberOnline.Conteudo.Domain.Entities;
using SaberOnline.Conteudo.Domain.Interfaces;
using SaberOnline.Conteudo.Domain.ValueObjects;
using SaberOnline.Core.Exceptions;
using SaberOnline.Core.SharedDto;

namespace SaberOnline.Conteudo.Tests.Applications;
public class AulaAppServiceTests
{
    #region Helpers
    private Curso CriarCurso(string nomeCurso = "Curso de DDD")
    {
        return new Curso(
            nomeCurso,
            1000m,
            DateTime.Now.AddMonths(6),
            new ConteudoProgramatico("Finalidade", new string('A', 60))
        );
    }

    private AulaDto CriarAulaDto() => new AulaDto
    {
        Descricao = "Aula de Introdução",
        CargaHoraria = 2,
        OrdemAula = 1,
        Ativo = true,
        Url = "http://google.com"
    };

    private AulaAppService CriarAppService(out Mock<IConteudoRepository> cursoRepoMock, Curso curso)
    {
        cursoRepoMock = new Mock<IConteudoRepository>();
        cursoRepoMock.Setup(r => r.ObterPorIdAsync(curso.Id)).ReturnsAsync(curso);
        cursoRepoMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(true);

        return new AulaAppService(cursoRepoMock.Object);
    }
    #endregion

    [Fact]
    public async Task Deve_adicionar_aula_com_sucesso()
    {
        // Arrange
        var curso = CriarCurso();
        var aulaDto = CriarAulaDto();
        var service = CriarAppService(out var mock, curso);

        // Act
        var aulaId = await service.AdicionarAulaAsync(curso.Id, aulaDto);

        // Assert
        curso.Aulas.Should().ContainSingle();
        aulaId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Deve_atualizar_aula_com_sucesso()
    {
        // Arrange
        var curso = CriarCurso();
        curso.AdicionarAula("Aula Original", 1, 1, "http://google.com");
        var aula = curso.Aulas.First();

        var dto = new AulaDto
        {
            Id = aula.Id,
            Descricao = "Aula Atualizada",
            CargaHoraria = 3,
            OrdemAula = 2,
            Ativo = false
        };

        var service = CriarAppService(out var mock, curso);

        // Act
        await service.AtualizarAulaAsync(curso.Id, dto);

        // Assert
        aula.Descricao.Should().Be("Aula Atualizada");
        aula.CargaHoraria.Should().Be(3);
        aula.OrdemAula.Should().Be(2);
        aula.Ativo.Should().BeFalse();
    }

    [Fact]
    public async Task Deve_remover_aula_com_sucesso()
    {
        // Arrange
        var curso = CriarCurso();
        curso.AdicionarAula("Aula a remover", 1, 1, "http://google.com");
        var aula = curso.Aulas.First();
        var service = CriarAppService(out var mock, curso);

        // Act
        await service.RemoverAulaAsync(curso.Id, aula.Id);

        // Assert
        curso.Aulas.Should().BeEmpty();
        mock.Verify(r => r.AtualizarAsync(curso), Times.Once);
        mock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task Deve_lancar_excecao_quando_aula_nao_encontrada()
    {
        // Arrange
        var curso = CriarCurso();
        var service = CriarAppService(out _, curso);
        var dto = CriarAulaDto();
        dto.Id = Guid.NewGuid(); // ID inexistente

        // Act
        Func<Task> act = async () => await service.AtualizarAulaAsync(curso.Id, dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Aula não encontrada*");
    }

    [Fact]
    public async Task Deve_lancar_excecao_quando_ordem_aula_ja_existir()
    {
        var curso = CriarCurso();
        curso.AdicionarAula("Aula existente 1", 2, 1, "http://google.com");
        curso.AdicionarAula("Aula existente 2", 4, 2, "http://google.com");

        var aulaNova = curso.Aulas.Last();
        var dto = new AulaDto
        {
            Id = aulaNova.Id,
            Descricao = "Atualizando aula Existente 2 para ordem 1",
            CargaHoraria = 2,
            OrdemAula = 1, // mesma ordem
            Ativo = true
        };

        var service = CriarAppService(out _, curso);

        Func<Task> act = async () => await service.AtualizarAulaAsync(curso.Id, dto);

        await act.Should().ThrowAsync<DomainException>()
                 .WithMessage("*Ordem da aula deve ser única dentro do curso*");
    }

    [Fact]
    public async Task Deve_lancar_excecao_quando_curso_nao_for_encontrado()
    {
        var repoMock = new Mock<IConteudoRepository>();
        repoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Curso?)null);
        var service = new AulaAppService(repoMock.Object);

        Func<Task> act = async () => await service.RemoverAulaAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Curso não encontrado*");
    }
}