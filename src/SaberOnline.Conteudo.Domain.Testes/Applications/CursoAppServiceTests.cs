using FluentAssertions;
using Moq;
using SaberOnline.Conteudo.Application.Services;
using SaberOnline.Conteudo.Application.ViewModels;
using SaberOnline.Conteudo.Domain.Entities;
using SaberOnline.Conteudo.Domain.Interfaces;
using SaberOnline.Conteudo.Domain.ValueObjects;
using SaberOnline.Core.Exceptions;


namespace SaberOnline.Conteudo.Tests.Applications;
public class CursoAppServiceTests
{
    #region Helpers
    private Curso CriarCurso() => new("Curso Teste", 1500m, DateTime.Now.AddMonths(6),
        new ConteudoProgramatico("Finalidade", new string('A', 60)));

    private CadastroCursoDto CriarCadastroDto() => new()
    {
        Nome = "Curso Teste",
        Valor = 1500m,
        ValidoAte = DateTime.Now.AddMonths(6),
        Finalidade = "Finalidade do Curso",
        Ementa = new string('A', 60)
    };

    private AtualizacaoCursoDto CriarAtualizacaoDto(Guid cursoId) => new()
    {
        Id = cursoId,
        Nome = "Curso Atualizado",
        Valor = 1800m,
        ValidoAte = DateTime.Now.AddMonths(12),
        Finalidade = "Finalidade Atualizada",
        Ementa = new string('B', 60),
        Ativo = true
    };

    private CursoAppService CriarAppService(out Mock<IConteudoRepository> repoMock, Curso curso = null)
    {
        repoMock = new Mock<IConteudoRepository>();

        repoMock.Setup(r => r.AdicionarAsync(It.IsAny<Curso>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.AtualizarAsync(It.IsAny<Curso>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.UnitOfWork.Commit()).ReturnsAsync(true);
        if (curso != null)
        {
            repoMock.Setup(r => r.ObterPorIdAsync(curso.Id)).ReturnsAsync(curso);
        }

        return new CursoAppService(repoMock.Object);
    }
    #endregion

    [Fact]
    public async Task Deve_cadastrar_curso_valido()
    {
        var service = CriarAppService(out var repoMock);
        var dto = CriarCadastroDto();

        var id = await service.CadastrarCursoAsync(dto);

        id.Should().NotBeEmpty();
        repoMock.Verify(r => r.AdicionarAsync(It.IsAny<Curso>()), Times.Once);
        repoMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task Nao_deve_cadastrar_curso_duplicado()
    {
        var repoMock = new Mock<IConteudoRepository>();
        repoMock.Setup(r => r.ExisteCursoComMesmoNomeAsync(It.IsAny<string>())).ReturnsAsync(true);

        var service = new CursoAppService(repoMock.Object);
        var dto = CriarCadastroDto();

        Func<Task> act = async () => await service.CadastrarCursoAsync(dto);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Já existe um curso cadastrado com esse nome*");
    }

    [Fact]
    public async Task Deve_atualizar_curso_valido()
    {
        var curso = CriarCurso();
        var service = CriarAppService(out var repoMock, curso);
        var dto = CriarAtualizacaoDto(curso.Id);

        await service.AtualizarCursoAsync(curso.Id, dto);

        curso.Nome.Should().Be(dto.Nome);
        curso.Valor.Should().Be(dto.Valor);
        curso.ConteudoProgramatico.Finalidade.Should().Be(dto.Finalidade);
        curso.ConteudoProgramatico.Ementa.Should().Be(dto.Ementa);

        repoMock.Verify(r => r.AtualizarAsync(curso), Times.Once);
        repoMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task Nao_deve_atualizar_curso_inexistente()
    {
        var service = CriarAppService(out var repoMock);
        var dto = CriarAtualizacaoDto(Guid.NewGuid());

        Func<Task> act = async () => await service.AtualizarCursoAsync(dto.Id, dto);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Curso não encontrado*");
    }

    [Fact]
    public async Task Deve_desativar_curso()
    {
        var curso = CriarCurso();
        var service = CriarAppService(out var repoMock, curso);

        await service.DesativarCursoAsync(curso.Id);

        curso.Ativo.Should().BeFalse();
        repoMock.Verify(r => r.AtualizarAsync(curso), Times.Once);
        repoMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task Deve_obter_curso_por_id()
    {
        var curso = CriarCurso();
        var service = CriarAppService(out var repoMock, curso);

        var dto = await service.ObterPorIdAsync(curso.Id);

        dto.Should().NotBeNull();
        dto.Id.Should().Be(curso.Id);
    }

    [Fact]
    public async Task Deve_obter_lista_de_cursos()
    {
        var curso = CriarCurso();
        var repoMock = new Mock<IConteudoRepository>();
        repoMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync([curso]);

        var service = new CursoAppService(repoMock.Object);

        var lista = await service.ObterTodosAsync();

        lista.Should().HaveCount(1);
    }

    [Fact]
    public async Task Nao_deve_atualizar_para_nome_ja_existente_em_outro_curso()
    {
        var cursoOriginal = CriarCurso();
        var dto = CriarAtualizacaoDto(cursoOriginal.Id);
        dto.Nome = "Curso Duplicado";

        var repoMock = new Mock<IConteudoRepository>();
        repoMock.Setup(r => r.ObterPorIdAsync(cursoOriginal.Id)).ReturnsAsync(cursoOriginal);
        repoMock.Setup(r => r.ExisteCursoComMesmoNomeAsync(dto.Nome)).ReturnsAsync(true);

        var service = new CursoAppService(repoMock.Object);

        Func<Task> act = async () => await service.AtualizarCursoAsync(cursoOriginal.Id, dto);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Já existe um curso cadastrado com esse nome*");
    }

    [Fact]
    public async Task Deve_obter_apenas_cursos_ativos()
    {
        var cursoAtivo = CriarCurso();
        var repoMock = new Mock<IConteudoRepository>();
        repoMock.Setup(r => r.ObterAtivosAsync()).ReturnsAsync([cursoAtivo]);

        var service = new CursoAppService(repoMock.Object);

        var lista = await service.ObterAtivosAsync();

        lista.Should().ContainSingle();
        lista.First().Id.Should().Be(cursoAtivo.Id);
    }
}
