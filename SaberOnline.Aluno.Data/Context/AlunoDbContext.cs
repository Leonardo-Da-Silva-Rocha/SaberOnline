using Microsoft.EntityFrameworkCore;
using SaberOnline.Core.Agregrates;
using System.Diagnostics.CodeAnalysis;

namespace SaberOnline.Aluno.Data.Context
{
    [ExcludeFromCodeCoverage]
    public class AlunoDbContext : DbContext, IUnitOfWork
    {
        public AlunoDbContext(DbContextOptions<AlunoDbContext> options) : base(options)
        {
        }

        public DbSet<Domain.Entities.Aluno> Alunos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AlunoDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
        public async Task<bool> Commit()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
