using SaberOnline.Core.SharedDto;

namespace SaberOnline.Core.Messages.Comunications.AlunoCommands;
public class ConcluirCursoCommand : CommandRaiz
{
    public Guid AlunoId { get; init; }
    public Guid MatriculaCursoId { get; init; }
    public CursoDto CursoDto { get; init; }

    public ConcluirCursoCommand(Guid alunoId, Guid matriculaCursoId, CursoDto cursoDto)
    {
        DefinirRaizAgregacao(alunoId);

        AlunoId = alunoId;
        MatriculaCursoId = matriculaCursoId;
        CursoDto = cursoDto;
    }
}
