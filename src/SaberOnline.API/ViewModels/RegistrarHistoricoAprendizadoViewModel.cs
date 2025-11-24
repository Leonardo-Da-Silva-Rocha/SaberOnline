namespace SaberOnline.API.ViewModels;
public class RegistrarHistoricoAprendizadoViewModel
{
    public Guid AlunoId { get; set; }
    public Guid MatriculaCursoId { get; set; }
    public Guid AulaId { get; set; }
    public DateTime? DataTermino { get; set; }
}
