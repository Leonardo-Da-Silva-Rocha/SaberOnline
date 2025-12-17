using FluentAssertions;
using SaberOnline.Conteudo.Domain.Entities;
using SaberOnline.Core.Exceptions;


namespace SaberOonline.Conteudo.Tests.Domains;
public class AulaTests
{
    #region Helpers
    private const string _cursoIdInvalido = "00000000-0000-0000-0000-000000000000";
    private const string _cursoIdValido = "11111111-1111-1111-1111-111111111111";
    private const string _descricaoValida = "Introdução ao Domain Driven Design";
    private const string _urlValida = "http://google.com";
    private const string _urlInvalida = "http:";
    private const int _cargaHorariaValida = 4;
    private const byte _ordemAulaValida = 1;

    private Aula CriarInstanciaAula(string cursoId = _cursoIdValido, 
        string descricao = _descricaoValida, 
        short cargaHoraria = _cargaHorariaValida, 
        byte ordemAula = _ordemAulaValida, 
        string url = _urlValida)
    {
        return new Aula(Guid.Parse(cursoId), descricao, cargaHoraria, ordemAula, url);
    }
    #endregion

    #region Construtores
    [Fact]
    public void Deve_criar_aula_valida()
    {
        // Act
        var aula = CriarInstanciaAula();

        // Assert
        aula.Should().NotBeNull();
        aula.Descricao.Should().Be(_descricaoValida);
        aula.CargaHoraria.Should().Be(_cargaHorariaValida);
        aula.OrdemAula.Should().Be(_ordemAulaValida);
        aula.CursoId.Should().Be(Guid.Parse(_cursoIdValido));
    }

    [Theory]
    [InlineData(_cursoIdInvalido, _descricaoValida, _cargaHorariaValida, _ordemAulaValida, _urlValida, "*Id do curso não pode ser vazio*")]
    [InlineData("11111111-1111-1111-1111-111111111111", "", _cargaHorariaValida, _ordemAulaValida, _urlValida, "*Descrição da aula não pode ser vazia ou nula*")]
    [InlineData(_cursoIdValido, "abc", _cargaHorariaValida, _ordemAulaValida, _urlValida, "*Descrição da aula deve ter entre 5 e 100 caracteres*")]
    [InlineData(_cursoIdValido, _descricaoValida, 0, _ordemAulaValida, _urlValida, "*Carga horária deve ser maior que zero*")]
    [InlineData(_cursoIdValido, _descricaoValida, 10, _ordemAulaValida, _urlValida, "*Carga horária deve estar entre 1 e 5 horas*")]
    [InlineData(_cursoIdValido, _descricaoValida, 2, 0, _urlValida, "*Ordem da aula deve ser maior que zero*")]
    [InlineData(_cursoIdValido, _descricaoValida, 2, 0, "", "*URL da aula não pode ser vazia ou nula*")]
    [InlineData(_cursoIdValido, _descricaoValida, _cargaHorariaValida, _ordemAulaValida, _urlInvalida, "*Url da aula deve ter entre 10 e 1024 caracteres*")]
    public void Nao_deve_criar_aula_invalida(string cursoId, string descricao, short cargaHoraria, byte ordemAula, string url, string mensagemErro)
    {
        // Arrange

        // Act
        Action act = () => CriarInstanciaAula(cursoId, descricao, cargaHoraria, ordemAula, url);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage(mensagemErro);
    }
    #endregion

    #region Metodos do Dominio
    [Fact]
    public void Deve_ativar_e_desativar_aula()
    {
        var aula = CriarInstanciaAula();

        aula.AtivarAula();
        aula.Ativo.Should().BeTrue();

        aula.DesativarAula();
        aula.Ativo.Should().BeFalse();
    }

    [Theory]
    [InlineData("", "*Descrição da aula não pode ser vazia ou nula*")]
    [InlineData("abc", "*Descrição da aula deve ter entre 5 e 100 caracteres*")]
    public void Nao_deve_alterar_descricao_invalida(string novaDescricao, string mensagemErro)
    {
        var aula = CriarInstanciaAula();

        Action act = () => aula.AlterarDescricao(novaDescricao);

        act.Should().Throw<DomainException>()
           .WithMessage(mensagemErro);
    }

    [Fact]
    public void Deve_alterar_descricao_valida()
    {
        // Arrange
        var novadescricao = "Introdução ao Domain Driven Design Atualizado";
        var aula = CriarInstanciaAula();

        // Act
        aula.AlterarDescricao(novadescricao);

        // Assert
        aula.Descricao.Should().Be(novadescricao);
    }

    [Theory]
    [InlineData(0, "*Carga horária deve ser maior que zero*")]
    [InlineData(6, "*Carga horária deve estar entre 1 e 5 horas*")]
    public void Nao_deve_alterar_carga_horaria_invalida(short novaCarga, string mensagemErro)
    {
        var aula = CriarInstanciaAula();

        Action act = () => aula.AlterarCargaHoraria(novaCarga);

        act.Should().Throw<DomainException>()
           .WithMessage(mensagemErro);
    }

    [Fact]
    public void Deve_alterar_carga_horaria_valida()
    {
        // Arrange
        short novaCargaHoraria = 5;
        var aula = CriarInstanciaAula();

        // Act
        aula.AlterarCargaHoraria(novaCargaHoraria);

        // Assert
        aula.CargaHoraria.Should().Be(novaCargaHoraria);
    }

    [Theory]
    [InlineData(0, "*Ordem da aula deve ser maior que zero*")]
    public void Nao_deve_alterar_ordem_invalida(byte novaOrdem, string mensagemErro)
    {
        var aula = CriarInstanciaAula();

        Action act = () => aula.AlterarOrdemAula(novaOrdem);

        act.Should().Throw<DomainException>()
           .WithMessage(mensagemErro);
    }

    [Fact]
    public void Deve_alterar_ordem_valida()
    {
        // Arrange
        byte novaOrdem = 10;
        var aula = CriarInstanciaAula();

        // Act
        aula.AlterarOrdemAula(novaOrdem);

        // Assert
        aula.OrdemAula.Should().Be(novaOrdem);
    }

    [Theory]
    [InlineData("", "*URL da aula não pode ser vazia ou nula*")]
    [InlineData("inv", "*Url da aula deve ter entre 10 e 1024 caracteres*")]
    public void Nao_deve_alterar_url_invalida(string novaUrl, string mensagemErro)
    {
        var aula = CriarInstanciaAula();

        Action act = () => aula.AlterarUrl(novaUrl);

        act.Should().Throw<DomainException>()
           .WithMessage(mensagemErro);
    }

    [Fact]
    public void Deve_alterar_url_valida()
    {
        // Arrange
        var novaUrl = "https://cursos.desenvolvedor.io/modulo-avancado-ddd";
        var aula = CriarInstanciaAula();

        // Act
        aula.AlterarUrl(novaUrl);

        // Assert
        aula.Url.Should().Be(novaUrl);
    }
    #endregion

    #region Testes de Override
    [Fact]
    public void ToString_deve_retornar_informacoes_da_aula()
    {
        var aula = CriarInstanciaAula();
        var resultado = aula.ToString();

        resultado.Should().Contain(_descricaoValida);
        resultado.Should().Contain(_cargaHorariaValida.ToString());
        resultado.Should().Contain(_ordemAulaValida.ToString());
    }

    #endregion
}
