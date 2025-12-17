using FluentAssertions;
using SaberOnline.Aluno.Domain.Entities;
using SaberOnline.Core.Exceptions;


namespace SaberOnline.Aluno.Tests.Domains;
public class AlunoTests
{
    #region Helpers
    private const string _nomeValido = "Jairo Azevedo";
    private const string _emailValido = "jsouza.lp@gmail.com";
    private static readonly DateTime _dataNascimentoValida = new(1973, 06, 25);

    private Domain.Entities.Aluno CriarAlunoValido(string nome = _nomeValido, string email = _emailValido, DateTime? dataNascimento = null)
    {
        return new Domain.Entities.Aluno(nome, email, dataNascimento ?? _dataNascimentoValida);
    }
    #endregion

    #region Construtores
    [Fact]
    public void Deve_criar_aluno_valido()
    {
        // Act
        var aluno = CriarAlunoValido();

        // Assert
        aluno.Nome.Should().Be(_nomeValido);
        aluno.Email.Should().Be(_emailValido);
        aluno.DataNascimento.Should().Be(_dataNascimentoValida);
    }

    [Theory]
    [InlineData("", "joao@email.com", "2000-01-01", "*Nome não pode ser nulo ou vazio*")]
    [InlineData("Jo", "joao@email.com", "2000-01-01", "*Nome deve ter entre 3 e 50 caracteres*")]
    [InlineData("João", "", "2000-01-01", "*Email não pode ser nulo ou vazio*")]
    [InlineData("João", "a@b", "2000-01-01", "*Email informado é inválido*")]
    [InlineData("João", "joao@email.com", "0001-01-01", "*Data de nascimento deve ser válida*")]
    [InlineData("João", "joao@email.com", "2099-01-01", "*Data de nascimento não pode ser superior à data atual*")]
    public void Nao_deve_criar_aluno_invalido(string nome, string email, string dataNascimentoStr, string mensagemErro)
    {
        // Arrange
        var dataNascimento = DateTime.Parse(dataNascimentoStr);

        // Act
        Action act = () => CriarAlunoValido(nome, email, dataNascimento);

        // Assert
        act.Should().Throw<DomainException>().WithMessage(mensagemErro);
    }

    [Fact]
    public void Nao_deve_criar_email_com_tamanho_maior_que_limite()
    {
        // Arrange
        var emailGrande = new string('a', 101) + "@email.com";

        // Act
        Action act = () => CriarAlunoValido("Fulano", emailGrande, DateTime.Now.AddYears(-20));

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Email deve ter entre 3 e 100 caracteres*");
    }
    #endregion

    #region Métodos do Domínio
    [Fact]
    public void Deve_atualizar_nome_valido()
    {
        // Arrange
        var aluno = CriarAlunoValido();
        var novoNome = "Maria Oliveira";

        // Act
        aluno.AtualizarNome(novoNome);

        // Assert
        aluno.Nome.Should().Be(novoNome);
    }

    [Theory]
    [InlineData("", "*Nome não pode ser nulo ou vazio*")]
    [InlineData("Jo", "*Nome deve ter entre 3 e 50 caracteres*")]
    public void Nao_deve_atualizar_nome_invalido(string novoNome, string mensagemErro)
    {
        // Arrange
        var aluno = CriarAlunoValido();

        // Act
        Action act = () => aluno.AtualizarNome(novoNome);

        // Assert
        act.Should().Throw<DomainException>().WithMessage(mensagemErro);
    }

    [Fact]
    public void Deve_atualizar_email_valido()
    {
        // Arrange
        var aluno = CriarAlunoValido();
        var novoEmail = "novo@email.com";

        // Act
        aluno.AtualizarEmail(novoEmail);

        // Assert
        aluno.Email.Should().Be(novoEmail);
    }

    [Theory]
    [InlineData("", "*Email não pode ser nulo ou vazio*")]
    [InlineData("x@x", "*Email informado é inválido*")]
    public void Nao_deve_atualizar_email_invalido(string novoEmail, string mensagemErro)
    {
        // Arrange
        var aluno = CriarAlunoValido();

        // Act
        Action act = () => aluno.AtualizarEmail(novoEmail);

        // Assert
        act.Should().Throw<DomainException>().WithMessage(mensagemErro);
    }

    [Fact]
    public void Deve_atualizar_data_nascimento_valida()
    {
        // Arrange
        var aluno = CriarAlunoValido();
        var novaData = new DateTime(1995, 5, 10);

        // Act
        aluno.AtualizarDataNascimento(novaData);

        // Assert
        aluno.DataNascimento.Should().Be(novaData);
    }

    [Fact]
    public void Nao_deve_atualizar_data_nascimento_futura()
    {
        // Arrange
        var aluno = CriarAlunoValido();
        var dataFutura = DateTime.Now.AddDays(1);

        // Act
        Action act = () => aluno.AtualizarDataNascimento(dataFutura);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Data de nascimento não pode ser superior à data atual*");
    }

    [Fact]
    public void Deve_matricular_em_curso()
    {
        // Arrange
        var aluno = CriarAlunoValido();
        var cursoId = Guid.NewGuid();
        var nomeCurso = "Curso de DDD";
        var valor = 900m;

        // Act
        aluno.MatricularEmCurso(cursoId, nomeCurso, valor);

        // Assert
        aluno.MatriculasCursos.Should().ContainSingle();
        var matricula = aluno.MatriculasCursos.First();
        matricula.CursoId.Should().Be(cursoId);
        matricula.NomeCurso.Should().Be(nomeCurso);
        matricula.Valor.Should().Be(valor);
    }

    [Fact]
    public void Nao_deve_matricular_em_curso_com_valor_invalido()
    {
        // Arrange
        var aluno = CriarAlunoValido();
        var cursoId = Guid.NewGuid();

        // Act
        Action act = () => aluno.MatricularEmCurso(cursoId, "Curso com valor zero", 0);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Valor da matrícula deve ser maior que zero*");
    }

    [Fact]
    public void Deve_obter_matricula_por_curso_id()
    {
        // Arrange
        var aluno = CriarAlunoValido();
        var cursoId = Guid.NewGuid();
        aluno.MatricularEmCurso(cursoId, "Curso de DDD", 1000);

        var matriculaId = aluno.MatriculasCursos.First().Id;

        // Act
        var matricula = aluno.ObterMatriculaCursoPeloId(matriculaId);

        // Assert
        matricula.Should().NotBeNull();
        matricula.CursoId.Should().Be(cursoId);
        matricula.Id.Should().Be(matriculaId);
    }

    [Fact]
    public void Nao_deve_retornar_matricula_inexistente_pelo_cursoId()
    {
        // Arrange
        var aluno = CriarAlunoValido();

        // Act
        Action act = () => aluno.ObterMatriculaPorCursoId(Guid.NewGuid());

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Matrícula pelo Curso não foi localizada*");
    }

    [Fact]
    public void Nao_deve_retornar_matricula_inexistente_pela_matriculaId()
    {
        // Arrange
        var aluno = CriarAlunoValido();

        // Act
        Action act = () => aluno.ObterMatriculaCursoPeloId(Guid.NewGuid());

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Matrícula não foi localizada*");
    }

    [Fact]
    public void Deve_permitir_multiplas_matriculas()
    {
        // Arrange
        var aluno = CriarAlunoValido();
        var curso1 = Guid.NewGuid();
        var curso2 = Guid.NewGuid();

        // Act
        aluno.MatricularEmCurso(curso1, "Curso Introdução ao DDD", 500);
        aluno.MatricularEmCurso(curso2, "Curso Avançado de DDD", 800);

        // Assert
        aluno.MatriculasCursos.Should().HaveCount(2);
    }

    [Fact]
    public void Nao_deve_permitir_matricula_duplicada_no_mesmo_curso()
    {
        var aluno = CriarAlunoValido();
        var cursoId = Guid.NewGuid();

        aluno.MatricularEmCurso(cursoId, "Curso Introdução ao DDD", 500);

        Action act = () => aluno.MatricularEmCurso(cursoId, "Curso Introdução ao DDD - Repetido", 500);

        act.Should().Throw<DomainException>()
            .WithMessage("*Aluno já está matriculado neste curso*");
    }

    [Fact]
    public void Deve_solicitar_certificado_quando_matricula_concluida()
    {
        var aluno = CriarAlunoValido();
        var cursoId = Guid.NewGuid();
        aluno.MatricularEmCurso(cursoId, "Curso Introdução ao DDD", 500);
        var matricula = aluno.ObterMatriculaPorCursoId(cursoId);
        aluno.AtualizarPagamentoMatricula(matricula.Id);

        aluno.ConcluirCurso(matricula.Id);

        aluno.RequisitarCertificadoConclusao(matricula.Id, "/var/tmp/certificados/JairoSouza.pdf");

        matricula.Certificado.Should().NotBeNull();
    }

    [Fact]
    public void Nao_deve_solicitar_certificado_quando_matricula_nao_concluida()
    {
        var aluno = CriarAlunoValido();
        aluno.MatricularEmCurso(Guid.NewGuid(), "Curso Introdução ao DDD", 500);
        var matricula = aluno.MatriculasCursos.First();

        Action act = () => aluno.RequisitarCertificadoConclusao(matricula.Id, "/var/tmp/certificados/JairoSouza.pdf");

        act.Should().Throw<DomainException>().WithMessage("*Certificado só pode ser solicitado após a conclusão do curso*");
    }

    [Fact]
    public void Deve_atualizar_pagamento_matricula_existente()
    {
        var aluno = CriarAlunoValido();
        aluno.MatricularEmCurso(Guid.NewGuid(), "Curso Introdução ao DDD", 500);
        var matricula = aluno.MatriculasCursos.First();

        aluno.AtualizarPagamentoMatricula(matricula.Id);

        matricula.EstadoMatricula.Should().Be(EstadoMatriculaCursoEnum.PagamentoRealizado);
    }

    [Fact]
    public void Nao_deve_atualizar_pagamento_matricula_inexistente()
    {
        var aluno = CriarAlunoValido();

        Action act = () => aluno.AtualizarPagamentoMatricula(Guid.NewGuid());

        act.Should().Throw<DomainException>().WithMessage("*Matrícula não foi localizada*");
    }

    [Fact]
    public void Deve_registrar_historico_aprendizado_para_matricula_existente()
    {
        var aluno = CriarAlunoValido();
        var cursoId = Guid.NewGuid();
        aluno.MatricularEmCurso(cursoId, "Curso Introdução ao DDD", 500);
        var matricula = aluno.ObterMatriculaPorCursoId(cursoId);
        aluno.AtualizarPagamentoMatricula(matricula.Id);

        aluno.RegistrarHistoricoAprendizado(matricula.Id, Guid.NewGuid(), "Curso Introdução ao DDD", DateTime.Now.Date);

        matricula.HistoricoAprendizado.Should().NotBeEmpty();
    }

    [Fact]
    public void Nao_deve_registrar_historico_aprendizado_para_matricula_inexistente()
    {
        var aluno = CriarAlunoValido();

        Action act = () => aluno.RegistrarHistoricoAprendizado(Guid.NewGuid(), Guid.NewGuid(), "Curso Introdução ao DDD", DateTime.Now.AddDays(-1));

        act.Should().Throw<DomainException>().WithMessage("*Matrícula não foi localizada*");
    }
    #endregion

    #region Overrides
    [Fact]
    public void ToString_deve_retornar_nome_e_email()
    {
        // Arrange
        var aluno = CriarAlunoValido();

        // Act
        var texto = aluno.ToString();

        // Assert
        texto.Should().Contain(_nomeValido);
        texto.Should().Contain(_emailValido);
    }
    #endregion
}
