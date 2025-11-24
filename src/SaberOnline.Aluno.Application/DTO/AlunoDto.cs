using SaberOnline.Core.SharedDto;

namespace SaberOnline.Aluno.Application.DTO;
public class AlunoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public DateTime DataNascimento { get; set; }

    public ICollection<MatriculaCursoDto> MatriculasCursos { get; set; }
}