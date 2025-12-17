using SaberOnline.Conteudo.Application.ViewModels;
using SaberOnline.Core.SharedDto;


namespace SaberOnline.Conteudo.Application.Services
{
    public interface ICursoAppService
    {
        Task<Guid> CadastrarCursoAsync(CadastroCursoDto dto);
        Task AtualizarCursoAsync(Guid cursoId, AtualizacaoCursoDto dto);
        Task DesativarCursoAsync(Guid cursoId);
        Task<CursoDto> ObterPorIdAsync(Guid cursoId);
        Task<IEnumerable<CursoDto>> ObterTodosAsync();
        Task<IEnumerable<CursoDto>> ObterAtivosAsync();
    }
}
