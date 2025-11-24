using FluentAssertions;
using SaberOnline.Aluno.Domain.Entities;
using SaberOnline.Core.Exceptions;

namespace SaberOnline.Aluno.Tests.Domains;
public class CertificadoTests
{
    #region Helpers
    private const string _matriculaCursoIdValido = "11111111-1111-1111-1111-111111111111";
    private const string _matriculaCursoIdInvalido = "00000000-0000-0000-0000-000000000000";
    private const string _pathValido = "/certificados/matricula-curso-id-certificado.pdf";

    private Certificado CriarCertificado(string matriculaCursoId = _matriculaCursoIdValido, string pathCertificado = _pathValido)
    {
        return new Certificado(Guid.Parse(matriculaCursoId), pathCertificado);
    }
    #endregion

    #region Construtores
    [Fact]
    public void Deve_criar_certificado_valido()
    {
        // Act
        var certificado = CriarCertificado();

        // Assert
        certificado.Should().NotBeNull();
        certificado.MatriculaCursoId.Should().Be(Guid.Parse(_matriculaCursoIdValido));
        certificado.PathCertificado.Should().Be(_pathValido);
        certificado.DataSolicitacao.Date.Should().Be(DateTime.Now.Date);
    }

    [Theory]
    [InlineData(_matriculaCursoIdInvalido, _pathValido, "*Matrícula do curso deve ser informada*")]
    [InlineData(_matriculaCursoIdValido, "", "*Path do certificado não pode ser nulo ou vazio*")]
    public void Nao_deve_criar_certificado_invalido(string matriculaCursoId, string path, string mensagemErro)
    {
        // Act
        Action act = () => CriarCertificado(matriculaCursoId, path);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage(mensagemErro);
    }

    [Fact]
    public void Nao_deve_criar_certificado_com_path_muito_longo()
    {
        var pathLongo = new string('x', 1025);

        Action act = () => CriarCertificado(_matriculaCursoIdValido, pathLongo);

        act.Should().Throw<DomainException>()
            .WithMessage("*Path do certificado deve ter no máximo 1024 caracteres*");
    }
    #endregion

    #region Metodos do Dominio
    #endregion

    #region Overrides
    [Fact]
    public void ToString_deve_retornar_descricao_formatada()
    {
        var certificado = CriarCertificado();
        var texto = certificado.ToString();

        texto.Should().Contain("Certificado da matrícula")
              .And.Contain(certificado.MatriculaCursoId.ToString("D"));
    }
    #endregion
}
