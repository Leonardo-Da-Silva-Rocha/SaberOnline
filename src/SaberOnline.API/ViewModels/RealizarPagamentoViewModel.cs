namespace SaberOnline.API.ViewModels;
public class RealizarPagamentoViewModel
{
    public Guid AlunoId { get; set; }
    public Guid MatriculaCursoId { get; set; }
    public decimal Valor { get; set; }
    public string NumeroCartao { get; set; }
    public string NomeTitularCartao { get; set; }
    public string ValidadeCartao { get; set; }
    public string CvvCartao { get; set; }
}
