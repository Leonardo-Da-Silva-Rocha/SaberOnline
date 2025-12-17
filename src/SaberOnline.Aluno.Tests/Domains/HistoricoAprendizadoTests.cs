using FluentAssertions;
using SaberOnline.Aluno.Domain.ValueObjects;
using SaberOnline.Core.Exceptions;

namespace SaberOnline.Aluno.Tests.Domains;
public class HistoricoAprendizadoTests
{
    #region Helpers
    private const string _matriculaIdValido = "11111111-1111-1111-1111-111111111111";
    private const string _cursoIdValido = "11111111-1111-1111-1111-111111111111";
    private const string _cursoIdInvalido = "00000000-0000-0000-0000-000000000000";
    private const string _aulaIdValido = "22222222-2222-2222-2222-222222222222";
    private const string _aulaIdInvalido = "00000000-0000-0000-0000-000000000000";
    private const string _nomeAulaValido = "Introdução aos fundamentos do DDD";

    private HistoricoAprendizado CriarHistorico(string matriculaId = _matriculaIdValido, 
        string cursoId = _cursoIdValido,
        string aulaId = _aulaIdValido,
        string nomeAula = _nomeAulaValido,
        DateTime? dataTermino = null)
    {
        return new HistoricoAprendizado(Guid.Parse(matriculaId),
            Guid.Parse(cursoId),
            Guid.Parse(aulaId),
            nomeAula,
            dataTermino);
    }
    #endregion

    #region Construtores
    [Fact]
    public void Deve_criar_historico_valido_sem_data_termino()
    {
        // Act
        var historico = CriarHistorico();

        // Assert
        historico.Should().NotBeNull();
        historico.CursoId.Should().Be(Guid.Parse(_cursoIdValido));
        historico.AulaId.Should().Be(Guid.Parse(_aulaIdValido));
        historico.NomeAula.Should().Be(_nomeAulaValido);
        historico.DataTermino.Should().BeNull();
        historico.DataInicio.Date.Should().Be(DateTime.Now.Date);
    }

    [Fact]
    public void Deve_criar_historico_valido_com_data_termino()
    {
        // Arrange
        var dataTermino = DateTime.Now;

        // Act
        var historico = CriarHistorico(dataTermino: dataTermino);

        // Assert
        historico.DataTermino.Should().Be(dataTermino);
    }

    [Theory]
    [InlineData(_matriculaIdValido, _cursoIdInvalido, _aulaIdValido, _nomeAulaValido, "*curso não pode ser vazio*")]
    [InlineData(_matriculaIdValido, _cursoIdValido, _aulaIdInvalido, _nomeAulaValido, "*aula não pode ser vazio*")]
    [InlineData(_matriculaIdValido, _cursoIdValido, _aulaIdValido, "", "*Nome da aula não pode ser vazio*")]
    [InlineData(_matriculaIdValido, _cursoIdValido, _aulaIdValido, "abc", "*Nome da aula deve ter entre 5 e 100 caracteres*")]
    public void Nao_deve_criar_historico_com_dados_invalidos(string matriculaIdStr, string cursoIdStr, string aulaIdStr, string nomeAula, string mensagemErro)
    {
        // Arrange
        var matriculaId = Guid.Parse(matriculaIdStr);
        var cursoId = Guid.Parse(cursoIdStr);
        var aulaId = Guid.Parse(aulaIdStr);

        // Act
        Action act = () => new HistoricoAprendizado(matriculaId, cursoId, aulaId, nomeAula);

        // Assert
        act.Should().Throw<DomainException>().WithMessage(mensagemErro);
    }

    [Fact]
    public void Nao_deve_criar_historico_com_data_termino_anterior_ao_inicio()
    {
        // Arrange
        var dataTermino = DateTime.Now.AddDays(-1);

        // Act
        Action act = () => CriarHistorico(dataTermino: dataTermino);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Data de término não pode ser menor que a data de início*");
    }

    [Fact]
    public void Nao_deve_criar_historico_com_data_termino_no_futuro()
    {
        // Arrange
        var dataTermino = DateTime.Now.AddDays(1);

        // Act
        Action act = () => CriarHistorico(dataTermino: dataTermino);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Data de término não pode ser superior à data atual*");
    }
    #endregion

    #region ToString
    [Fact]
    public void ToString_deve_retornar_texto_formatado()
    {
        // Arrange
        var historico = CriarHistorico();

        // Act
        var texto = historico.ToString();

        // Assert
        texto.Should().Contain("Aula");
        texto.Should().Contain("Iniciada em");
    }
    #endregion
}