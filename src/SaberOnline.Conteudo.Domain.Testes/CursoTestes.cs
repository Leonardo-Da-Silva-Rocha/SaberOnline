using SaberOnline.Conteudo.Domain.Entities;
using SaberOnline.Conteudo.Domain.ValueObjects;
using SaberOnline.Core.Exceptions;

namespace SaberOnline.Conteudo.Domain.Testes
{
    public class CursoTestes
    {
        [Fact]
        public void Curso_Validar_ValidacoesDevemRetornarExceptions()
        {

            // Arrange & Act & Assert
            var ex = Assert.Throws<DomainException>(() =>
                new Curso(string.Empty, 1, DateTime.Now, new ConteudoProgramatico("finalidade","ementa"))
            );

            Assert.Equal("O campo Nome do curso não pode estar vazio", ex.Message);
        }
    }
}