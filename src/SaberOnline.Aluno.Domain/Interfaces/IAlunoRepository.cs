
using SaberOnline.Core.Data;
namespace SaberOnline.Aluno.Domain.Interfaces
{
    public interface IAlunoRepository : IRepository<Entities.Aluno>
    {
        Task AdicionarAsync(Entities.Aluno aluno);
        Task AtualizarAsync(Entities.Aluno aluno);
    }
}