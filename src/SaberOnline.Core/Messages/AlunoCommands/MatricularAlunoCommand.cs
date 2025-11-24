namespace SaberOnline.Core.Messages.AlunoCommands;
public class MatricularAlunoCommand : CommandRaiz
{
    public Guid AlunoId { get; init; }
    public Guid CursoId { get; init; }
    public bool CursoDisponivel { get; init; }
    public string NomeCurso { get; init; } 
    public decimal ValorCurso { get; init; }

    public MatricularAlunoCommand(Guid alunoId, Guid cursoId, bool cursoDisponivel, string nomeCurso, decimal valorCurso)
    {
        DefinirRaizAgregacao(alunoId);

        AlunoId = alunoId;
        CursoId = cursoId;
        CursoDisponivel = cursoDisponivel;
        NomeCurso = nomeCurso;
        ValorCurso = valorCurso;
    }
}
