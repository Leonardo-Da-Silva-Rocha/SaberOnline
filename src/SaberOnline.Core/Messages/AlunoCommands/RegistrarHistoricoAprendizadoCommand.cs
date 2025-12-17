using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoCommands;
using SaberOnline.Core.SharedDto;

namespace SaberOnline.Core.Messages.Comunications.AlunoCommands;
public class RegistrarHistoricoAprendizadoCommand : CommandRaiz
{
    public Guid AlunoId { get; private set; }
    public Guid MatriculaCursoId { get; private set; }
    public Guid AulaId { get; private set; }
    public DateTime? DataTermino { get; private set; }
    public CursoDto CursoDto { get; private set; }

    public RegistrarHistoricoAprendizadoCommand(Guid alunoId, Guid matriculaCursoId, Guid aulaId, CursoDto cursoDto, DateTime? dataTermino = null)
    {
        DefinirRaizAgregacao(alunoId);

        AlunoId = alunoId;
        MatriculaCursoId = matriculaCursoId;
        AulaId = aulaId;
        DataTermino = dataTermino;
        CursoDto = cursoDto;
    }
}