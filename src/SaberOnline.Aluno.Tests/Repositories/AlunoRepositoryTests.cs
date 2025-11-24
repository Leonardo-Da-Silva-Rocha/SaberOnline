using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SaberOnline.Aluno.Data.Context;
using SaberOnline.Aluno.Data.Repositories;
using SaberOnline.Aluno.Domain.Entities;


namespace SaberOnline.Aluno.Tests.Repositories;
public class AlunoRepositoryTests
{
    #region Helpers
    private Domain.Entities.Aluno CriarAlunoValido()
    {
        return new Domain.Entities.Aluno("Jairo Azevedo", "jairoSouza@email.com", new DateTime(1973, 06, 25));
    }

    private AlunoRepository CriarRepository(out AlunoDbContext context)
    {
        var options = new DbContextOptionsBuilder<AlunoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        context = new AlunoDbContext(options);
        var a = new AlunoRepository(context);
        return a;
    }
    #endregion

    #region Ações na tabela - Aluno
    [Fact]
    public async Task Deve_adicionar_e_buscar_aluno_por_id()
    {
        var aluno = CriarAlunoValido();
        var repository = CriarRepository(out var context);

        await repository.AdicionarAsync(aluno);
        await repository.UnitOfWork.Commit();
        //context.SaveChangesAsync();

        var alunoDb = await repository.ObterPorIdAsync(aluno.Id);

        alunoDb.Should().NotBeNull();
        alunoDb.Email.Should().Be(aluno.Email);
    }

    [Fact]
    public async Task Deve_retornar_aluno_por_email()
    {
        var aluno = CriarAlunoValido();
        var repository = CriarRepository(out var context);

        await repository.AdicionarAsync(aluno);
        await repository.UnitOfWork.Commit();
        

        var alunoDb = await repository.ObterPorEmailAsync(aluno.Email);

        alunoDb.Should().NotBeNull();
        alunoDb.Nome.Should().Be(aluno.Nome);
    }

    [Fact]
    public async Task Deve_verificar_existencia_de_email()
    {
        var aluno = CriarAlunoValido();
        var repository = CriarRepository(out var context);

        await repository.AdicionarAsync(aluno);
        await repository.UnitOfWork.Commit();
        

        var existe = await repository.ExisteEmailAsync(aluno.Email);
        existe.Should().BeTrue();
    }

    [Fact]
    public async Task Nao_deve_encontrar_aluno_com_email_inexistente()
    {
        var repository = CriarRepository(out var _);

        var aluno = await repository.ObterPorEmailAsync("email@inexistente.com");
        aluno.Should().BeNull();
    }

    [Fact]
    public async Task Deve_atualizar_aluno()
    {
        var aluno = CriarAlunoValido();
        var repository = CriarRepository(out var context);

        await repository.AdicionarAsync(aluno);
        await repository.UnitOfWork.Commit();
        //await context.SaveChangesAsync();

        aluno.AtualizarNome("Jairo Atualizado");
        await repository.AtualizarAsync(aluno);
        await context.SaveChangesAsync();

        var alunoAtualizado = await repository.ObterPorIdAsync(aluno.Id);
        alunoAtualizado.Nome.Should().Be("Jairo Atualizado");
    }
    #endregion

    #region Ações na tabela - Matricula
    [Fact]
    public async Task Deve_obter_matricula_por_id()
    {
        var repository = CriarRepository(out var context);
        var aluno = CriarAlunoValido();
        var matricula = new MatriculaCurso(aluno.Id, Guid.NewGuid(), "Curso de Gestão de Processos", 800);

        context.Alunos.Add(aluno);
        context.MatriculasCursos.Add(matricula);
        await repository.UnitOfWork.Commit();
        //await context.SaveChangesAsync();

        var resultado = await repository.ObterMatriculaPorIdAsync(matricula.Id);
        resultado.Should().NotBeNull();
        resultado.CursoId.Should().Be(matricula.CursoId);
    }

    [Fact]
    public async Task Deve_obter_matricula_por_aluno_e_curso()
    {
        var repository = CriarRepository(out var context);
        var aluno = CriarAlunoValido();
        var cursoId = Guid.NewGuid();
        var matricula = new MatriculaCurso(aluno.Id, cursoId, "Curso de Gestão de Processos", 500);

        context.Alunos.Add(aluno);
        context.MatriculasCursos.Add(matricula);
        await repository.UnitOfWork.Commit();
        //await context.SaveChangesAsync();

        var resultado = await repository.ObterMatriculaPorAlunoECursoAsync(aluno.Id, cursoId);
        resultado.Should().NotBeNull();
        resultado.CursoId.Should().Be(cursoId);
    }

    [Fact]
    public async Task Deve_adicionar_certificado_e_matricula()
    {
        var repo = CriarRepository(out var context);
        var aluno = CriarAlunoValido();
        var matricula = new MatriculaCurso(aluno.Id, Guid.NewGuid(), "Curso de TDD - Iniciante", 100);
        var certificado = new Certificado(matricula.Id, "tmp/certificado.pdf");

        context.Alunos.Add(aluno);
        await repo.AdicionarMatriculaCursoAsync(matricula);
        await repo.AdicionarCertificadoMatriculaCursoAsync(certificado);
        await repo.UnitOfWork.Commit();

        var certificadoDb = await context.Certificados.FirstOrDefaultAsync();
        var matriculaDb = await context.MatriculasCursos.FirstOrDefaultAsync();

        certificadoDb.Should().NotBeNull();
        matriculaDb.Should().NotBeNull();
    }
    #endregion

    #region Ações na tabela - Certificado
    [Fact]
    public async Task Deve_obter_certificado_por_matricula()
    {
        var repository = CriarRepository(out var context);
        var aluno = CriarAlunoValido();
        var matricula = new MatriculaCurso(aluno.Id, Guid.NewGuid(), "Curso de Orientação a dados", 1250m);
        var certificado = new Certificado(matricula.Id, "var/tmp/certificados/certificado.pdf");

        context.Alunos.Add(aluno);
        context.MatriculasCursos.Add(matricula);
        context.Certificados.Add(certificado);
        await repository.UnitOfWork.Commit();
        

        var resultado = await repository.ObterCertificadoPorMatriculaAsync(matricula.Id);
        resultado.Should().NotBeNull();
        resultado.PathCertificado.Should().Be(certificado.PathCertificado);
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
    #endregion
}
