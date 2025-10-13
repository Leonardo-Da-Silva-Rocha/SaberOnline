using Microsoft.EntityFrameworkCore;
using SaberOnline.Aluno.Data.Context;
using SaberOnline.Autenticacao.Data;

namespace SaberOnline.API.MigrationHelper
{
    public static class DbMigrationHelper
    {
        private static AlunoDbContext _alunoContext = null;
        private static AutenticacaoDbContext _identityContext = null;

        public static async Task AutocarregamentoDadosAsync(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await CarregamentoDadosAsync(services);
        }

        public static async Task CarregamentoDadosAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            _identityContext = scope.ServiceProvider.GetRequiredService<AutenticacaoDbContext>();

            _alunoContext = scope.ServiceProvider.GetRequiredService<AlunoDbContext>();

            if (env.IsDevelopment())
            {
                await _identityContext.Database.MigrateAsync();
                await _alunoContext.Database.MigrateAsync();
            }
        }
        //TODO: POPULAR RESTANTE DO BANCO QUANDO TIVER MAIS ENTIDADES
    }
}