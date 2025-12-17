using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SaberOnline.Conteudo.Data;
using SaberOnline.Conteudo.Data.Repository;
using SaberOnline.Conteudo.Domain.Entities;
using SaberOnline.Conteudo.Domain.ValueObjects;


namespace SaberOnline.Conteudo.Tests.Repositories;
public class CursoRepositoryTests
{
    #region Helpers
    private Curso CriarCursoValido(string nome = "Curso de DDD")
    {
        return new Curso(
            nome,
            1200m,
            DateTime.Now.AddMonths(3),
            new ConteudoProgramatico("Especialização", new string('A', 60))
        );
    }
    #endregion

    #region Construtores
    private ConteudoRepository CriarRepository(out ConteudoContext context)
    {
        var options = new DbContextOptionsBuilder<ConteudoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        context = new ConteudoContext(options);
        return new ConteudoRepository(context);
    }
    #endregion

    #region Ações na tabela
    [Fact]
    public async Task Deve_adicionar_e_buscar_curso_por_id()
    {
        // Arrange
        var curso = CriarCursoValido();
        var repository = CriarRepository(out var context);

        // Act
        await repository.AdicionarAsync(curso);
        await repository.UnitOfWork.Commit();

        var cursoDb = await repository.ObterPorIdAsync(curso.Id);

        // Assert
        cursoDb.Should().NotBeNull();
        cursoDb.Id.Should().Be(curso.Id);
    }

    [Fact]
    public async Task Deve_retornar_todos_os_cursos()
    {
        var curso1 = CriarCursoValido("Curso de Refatorações");
        var curso2 = CriarCursoValido("Curso de como não abusar demais no DDD");
        var repository = CriarRepository(out var context);

        await repository.AdicionarAsync(curso1);
        await repository.AdicionarAsync(curso2);
        await repository.UnitOfWork.Commit();

        var cursos = await repository.ObterTodosAsync();

        cursos.Should().HaveCount(2);
    }

    [Fact]
    public async Task Deve_retornar_somente_cursos_ativos()
    {
        var curso1 = CriarCursoValido("Curso de Refatorações");
        curso1.AtivarCurso();
        var curso2 = CriarCursoValido("Curso de como não abusar demais no DDD");
        curso2.DesativarCurso();

        var repository = CriarRepository(out var context);

        await repository.AdicionarAsync(curso1);
        await repository.AdicionarAsync(curso2);
        await repository.UnitOfWork.Commit();

        var cursosAtivos = await repository.ObterAtivosAsync();

        cursosAtivos.Should().ContainSingle(c => c.Id == curso1.Id);
    }

    [Fact]
    public async Task Deve_retornar_true_quando_existir_curso_com_mesmo_nome()
    {
        var curso = CriarCursoValido("Curso Duplicado");
        var repository = CriarRepository(out var context);

        await repository.AdicionarAsync(curso);
        await repository.UnitOfWork.Commit();

        var existe = await repository.ExisteCursoComMesmoNomeAsync("Curso Duplicado");

        existe.Should().BeTrue();
    }

    [Fact]
    public async Task Deve_retornar_false_quando_nao_existir_curso_com_nome()
    {
        var repository = CriarRepository(out var _);

        var existe = await repository.ExisteCursoComMesmoNomeAsync("NomeInexistente");

        existe.Should().BeFalse();
    }

    [Fact]
    public async Task Deve_atualizar_curso()
    {
        // Arrange
        var curso = CriarCursoValido("Curso de como não abusar demais no DDD");
        var repository = CriarRepository(out var context);
        await repository.AdicionarAsync(curso);
        await repository.UnitOfWork.Commit();

        // Act
        curso.AlterarNome("Curso de como não abusar demais no DDD Atualizado");
        await repository.AtualizarAsync(curso);
        await repository.UnitOfWork.Commit();

        var cursoAtualizado = await repository.ObterPorIdAsync(curso.Id);

        // Assert
        cursoAtualizado.Nome.Should().Be("Curso de como não abusar demais no DDD Atualizado");
    }

    [Fact]
    public async Task Deve_desativar_curso()
    {
        var curso = CriarCursoValido("Curso para desativar");
        var repository = CriarRepository(out var context);
        await repository.AdicionarAsync(curso);
        await repository.UnitOfWork.Commit();

        await repository.DesativarAsync(curso);
        await repository.UnitOfWork.Commit();

        var cursoDesativado = await repository.ObterPorIdAsync(curso.Id);
        cursoDesativado.Ativo.Should().BeFalse();
    }

    [Fact]
    public async Task Deve_retornar_null_para_id_inexistente()
    {
        var repository = CriarRepository(out var _);
        var curso = await repository.ObterPorIdAsync(Guid.NewGuid());

        curso.Should().BeNull();
    }

    [Fact]
    public async Task Deve_retornar_true_se_houver_cursos_com_nomes_iguais()
    {
        var curso1 = CriarCursoValido("Curso Duplicado...");
        var curso2 = CriarCursoValido("Curso Duplicado...");

        var repository = CriarRepository(out var context);
        await repository.AdicionarAsync(curso1);
        await repository.AdicionarAsync(curso2);
        await repository.UnitOfWork.Commit();

        var existe = await repository.ExisteCursoComMesmoNomeAsync("Curso Duplicado...");

        existe.Should().BeTrue();
    }
    #endregion

    #region Overrides
    [Fact]
    public void Dispose_deve_encerrar_contexto_sem_excecao()
    {
        var repository = CriarRepository(out var context);
        var act = () => repository.Dispose();

        act.Should().NotThrow();
    }

    [Fact]
    public async Task Deve_adicionar_aula_ao_contexto()
    {
        // Arrange
        var curso = CriarCursoValido();
        
        var repository = CriarRepository(out var context);
        await repository.AdicionarAsync(curso);
        await repository.UnitOfWork.Commit();

        // Act
        curso.AdicionarAula("Aula de criptografia avançada", 2, 9, "https://jwt.io");
        Aula aulaCriada = curso.Aulas.Last();
        await repository.AdicionarAulaAsync(aulaCriada);
        await repository.UnitOfWork.Commit();

        var cursoDb = await repository.ObterPorIdAsync(curso.Id);

        // Assert
        cursoDb.Aulas.Should().ContainSingle(a => a.Id == aulaCriada.Id);
    }
    #endregion
}
