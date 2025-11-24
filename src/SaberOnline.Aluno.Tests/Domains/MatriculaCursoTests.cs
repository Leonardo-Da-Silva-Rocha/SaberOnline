using FluentAssertions;
using SaberOnline.Aluno.Domain.Entities;
using SaberOnline.Core.Exceptions;


namespace SaberOnline.Aluno.Tests.Domains;
public class MatriculaCursoTests
{
    #region Helpers
    private const string _alunoIdValido = "11111111-1111-1111-1111-111111111111";
    private const string _cursoIdValido = "22222222-2222-2222-2222-222222222222";
    private const string _nomeCursoValido = "Curso Completo de DDD";
    private const double _valorValido = 1000.00;

    private MatriculaCurso CriarMatricula(
        string alunoId = _alunoIdValido,
        string cursoId = _cursoIdValido,
        string nomeCurso = _nomeCursoValido,
        double valor = _valorValido)
    {
        return new MatriculaCurso(Guid.Parse(alunoId), Guid.Parse(cursoId), nomeCurso, (decimal)valor);
    }
    #endregion

    #region Construtores
    [Fact]
    public void Deve_criar_matricula_valida()
    {
        var matricula = CriarMatricula();

        matricula.Should().NotBeNull();
        matricula.EstadoMatricula.Should().Be(EstadoMatriculaCursoEnum.PendentePagamento);
        matricula.MatriculaCursoConcluido.Should().BeFalse();
        matricula.DataMatricula.Date.Should().Be(DateTime.Now.Date);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", _cursoIdValido, _nomeCursoValido, _valorValido, "*Aluno deve ser informado*")]
    [InlineData(_alunoIdValido, "00000000-0000-0000-0000-000000000000", _nomeCursoValido, _valorValido, "*Curso deve ser informado*")]
    [InlineData(_alunoIdValido, _cursoIdValido, "", _valorValido, "*Nome do curso deve ser informado*")]
    [InlineData(_alunoIdValido, _cursoIdValido, "abc", _valorValido, "*Nome do curso deve ter entre 10 e 100 caracteres*")]
    [InlineData(_alunoIdValido, _cursoIdValido, _nomeCursoValido, 0, "*Valor da matrícula deve ser maior que zero*")]
    public void Nao_deve_criar_matricula_invalida(string alunoId, string cursoId, string nomeCurso, decimal valor, string mensagemErro)
    {
        Action act = () => new MatriculaCurso(Guid.Parse(alunoId), Guid.Parse(cursoId), nomeCurso, valor);

        act.Should().Throw<DomainException>().WithMessage(mensagemErro);
    }
    #endregion

    #region Métodos do Domínio - Estados
    [Fact]
    public void Deve_atualizar_pagamento()
    {
        var matricula = CriarMatricula();

        matricula.AtualizarPagamentoMatricula();

        matricula.EstadoMatricula.Should().Be(EstadoMatriculaCursoEnum.PagamentoRealizado);
    }

    [Fact]
    public void Deve_concluir_curso_quando_pagamento_realizado()
    {
        var matricula = CriarMatricula();
        matricula.AtualizarPagamentoMatricula();

        matricula.ConcluirCurso();

        matricula.MatriculaCursoConcluido.Should().BeTrue();
        matricula.DataConclusao.Should().NotBeNull();
    }

    [Fact]
    public void Nao_deve_concluir_curso_quando_abandonado()
    {
        var matricula = CriarMatricula();
        matricula.AtualizarAbandonoMatricula();

        Action act = () => matricula.ConcluirCurso();

        act.Should().Throw<DomainException>().WithMessage("*Não é possível concluir um curso com estado de pagamento abandonado*");
    }

    [Fact]
    public void Nao_deve_abandonar_curso_concluido()
    {
        var matricula = CriarMatricula();
        matricula.AtualizarPagamentoMatricula();
        matricula.ConcluirCurso();

        Action act = () => matricula.AtualizarAbandonoMatricula();

        act.Should().Throw<DomainException>().WithMessage("*Não é possível alterar o estado da matrícula para pagamento abandonado com o curso concluído*");
    }
    #endregion

    #region Certificado
    [Fact]
    public void Deve_requisitar_certificado()
    {
        var aulaId1 = Guid.NewGuid();
        var aulaId2 = Guid.NewGuid();
        var dataTermino = DateTime.Today;
        var path = "/certificados/teste.pdf";

        var matricula = CriarMatricula();
        matricula.AtualizarPagamentoMatricula();

        matricula.RegistrarHistoricoAprendizado(aulaId1, "Aula de teste unitário 1", dataTermino);
        matricula.RegistrarHistoricoAprendizado(aulaId2, "Aula de teste unitário 2", dataTermino);

        matricula.ConcluirCurso();
        matricula.RequisitarCertificadoConclusao(path);

        matricula.Certificado.Should().NotBeNull();
        matricula.Certificado.PathCertificado.Should().Be(path);
    }

    [Fact]
    public void Nao_deve_requisitar_certificado_duplicado()
    {
        var aulaId1 = Guid.NewGuid();
        var aulaId2 = Guid.NewGuid();
        var dataTermino = DateTime.Today;

        var matricula = CriarMatricula();
        matricula.AtualizarPagamentoMatricula();

        matricula.RegistrarHistoricoAprendizado(aulaId1, "Aula de teste unitário 1", dataTermino);
        matricula.RegistrarHistoricoAprendizado(aulaId2, "Aula de teste unitário 2", dataTermino);

        matricula.ConcluirCurso();
        matricula.RequisitarCertificadoConclusao("/certificados/teste.pdf");

        Action act = () => matricula.RequisitarCertificadoConclusao("/certificados/novo.pdf");

        act.Should().Throw<DomainException>().WithMessage("*Certificado já foi solicitado para esta matrícula*");
    }
    #endregion

    #region Histórico de Aprendizado
    [Fact]
    public void Deve_registrar_historico_de_aula()
    {
        var matricula = CriarMatricula();
        matricula.AtualizarPagamentoMatricula();

        var aulaId = Guid.NewGuid();
        var nomeAula = "Aula Teste 0000001";

        matricula.RegistrarHistoricoAprendizado(aulaId, nomeAula);

        matricula.HistoricoAprendizado.Should().ContainSingle();
        matricula.ObterHistoricoAulaPeloId(aulaId).NomeAula.Should().Be(nomeAula);
    }

    [Fact]
    public void Nao_deve_reinserir_aula_concluida()
    {
        var matricula = CriarMatricula();
        matricula.AtualizarPagamentoMatricula();
        var aulaId = Guid.NewGuid();
        matricula.RegistrarHistoricoAprendizado(aulaId, "Aula teste 01", DateTime.Now);

        Action act = () => matricula.RegistrarHistoricoAprendizado(aulaId, "Aula teste 01");

        act.Should().Throw<DomainException>().WithMessage("*já foi concluída*");
    }

    [Fact]
    public void Deve_substituir_historico_em_andamento()
    {
        var matricula = CriarMatricula();
        matricula.AtualizarPagamentoMatricula();
        var aulaId = Guid.NewGuid();
        matricula.RegistrarHistoricoAprendizado(aulaId, "Aula de teste 01");
        matricula.RegistrarHistoricoAprendizado(aulaId, "Aula de teste 01", DateTime.Now);

        var historico = matricula.ObterHistoricoAulaPeloId(aulaId);
        historico.DataTermino.Should().NotBeNull();
    }

    [Fact]
    public void Nao_deve_retornar_historico_inexistente()
    {
        var matricula = CriarMatricula();

        Action act = () => matricula.ObterHistoricoAulaPeloId(Guid.NewGuid());

        act.Should().Throw<DomainException>().WithMessage("*não foi localizado*");
    }

    [Fact]
    public void Deve_retornar_true_quando_matricula_pode_ser_finalizada()
    {
        // Arrange
        var aulaId1 = Guid.NewGuid();
        var aulaId2 = Guid.NewGuid();
        var dataTermino = DateTime.Today;

        var matricula = CriarMatricula();
        matricula.AtualizarPagamentoMatricula();

        matricula.RegistrarHistoricoAprendizado(aulaId1, "Aula de teste unitário 1", dataTermino);
        matricula.RegistrarHistoricoAprendizado(aulaId2, "Aula de teste unitário 2", dataTermino);

        // Act
        var podeFinalizar = matricula.PodeFinalizarMatriculaCurso;

        // Assert
        podeFinalizar.Should().BeTrue();
    }
    #endregion

    #region ToString
    [Fact]
    public void ToString_deve_retornar_texto_formatado()
    {
        var matricula = CriarMatricula();

        var texto = matricula.ToString();

        texto.Should().Contain("Matrícula no curso");
    }
    #endregion
}
