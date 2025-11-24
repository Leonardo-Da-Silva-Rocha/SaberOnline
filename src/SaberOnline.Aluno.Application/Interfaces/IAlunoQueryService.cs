using SaberOnline.Aluno.Application.DTO;
using SaberOnline.Core.SharedDto;

namespace SaberOnline.Aluno.Application.Interfaces;
public interface IAlunoQueryService 
{
    Task<AlunoDto> ObterAlunoPorIdAsync(Guid alunoId);
    Task<EvolucaoAlunoDto> ObterEvolucaoMatriculasCursoDoAlunoPorIdAsync(Guid alunoId);
    Task<IEnumerable<MatriculaCursoDto>> ObterMatriculasPorAlunoIdAsync(Guid alunoId);
    Task<MatriculaCursoDto> ObterInformacaoMatriculaCursoAsync(Guid matriculaCursoId);
    Task<CertificadoDto> ObterCertificadoPorMatriculaIdAsync(Guid matriculaCursoId);
    Task<IEnumerable<AulaCursoDto>> ObterAulasPorMatriculaIdAsync(Guid matriculaCursoId, CursoDto cursoDto);
}
