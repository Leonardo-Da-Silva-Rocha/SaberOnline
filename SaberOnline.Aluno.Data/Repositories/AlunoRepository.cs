using SaberOnline.Aluno.Data.Context;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core.Agregrates;

namespace SaberOnline.Aluno.Data.Repositories
{
    public class AlunoRepository(AlunoDbContext context) : IAlunoRepository
    {
        private readonly AlunoDbContext _context = context;
        public IUnitOfWork UnitOfWork => _context;

        public async Task AdicionarAsync(Domain.Entities.Aluno aluno)
        {
            await _context.Alunos.AddAsync(aluno);
        }

        public async Task AtualizarAsync(Domain.Entities.Aluno aluno)
        {
            _context.Alunos.Update(aluno);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}