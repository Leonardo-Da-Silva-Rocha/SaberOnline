namespace SaberOnline.Core.Messages.AlunoCommands;
public class AtualizarPagamentoMatriculaCommand : CommandRaiz
{
    public Guid AlunoId { get; init; }
    public Guid CursoId { get; init; }
    public bool CursoDisponivel { get; init; }

    public AtualizarPagamentoMatriculaCommand(Guid alunoId, Guid cursoId, bool cursoDisponivel)
    {
        DefinirRaizAgregacao(alunoId);

        AlunoId = alunoId;
        CursoId = cursoId;
        CursoDisponivel = cursoDisponivel;
    }
}
