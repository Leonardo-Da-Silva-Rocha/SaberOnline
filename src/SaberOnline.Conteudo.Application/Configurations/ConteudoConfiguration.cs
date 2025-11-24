using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaberOnline.Conteudo.Application.Services;
using SaberOnline.Conteudo.Data;
using SaberOnline.Conteudo.Data.Repository;
using SaberOnline.Conteudo.Domain.Interfaces;
using SaberOnline.Core;
using SaberOnline.Core.DomainHadlers;
using SaberOnline.Core.Messages;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SaberOnline.Conteudo.Application.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class ConteudoConfiguration
    {
        
        public static IServiceCollection ConfigurarConteudoApplication(this IServiceCollection services, string stringConexao, bool ehProducao)
        {
            return services
                .ConfigurarInjecoesDependenciasRepository()
                .ConfigurarInjecoesDependenciasApplication()
                .ConfigurarRepositorios(stringConexao, ehProducao);
        }

        private static IServiceCollection ConfigurarInjecoesDependenciasRepository(this IServiceCollection services)
        {
            services.AddScoped<IConteudoRepository, ConteudoRepository>();
            return services;
        }

        private static IServiceCollection ConfigurarInjecoesDependenciasApplication(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            services.AddScoped<INotificationHandler<DomainNotificacaoRaiz>, DomainNotificacaoHandler>();
            
            services.AddScoped<ICursoAppService, CursoAppService>();
            services.AddScoped<IAulaAppService, AulaAppService>();

            return services;
        }

        private static IServiceCollection ConfigurarRepositorios(this IServiceCollection services, string stringConexao, bool ehProducao)
        {
            services.AddDbContext<ConteudoContext>(o =>
            {
                if (ehProducao)
                {
                    o.UseSqlServer(stringConexao);
                }
                else
                {
                    var connection = new SqliteConnection(stringConexao);
                    connection.CreateCollation("LATIN1_GENERAL_CI_AI", (x, y) =>
                    {
                        if (x == null && y == null) return 0;
                        if (x == null) return -1;
                        if (y == null) return 1;

                        return string.Compare(x, y, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
                    });

                    o.UseSqlite(connection);
                }
            });

            return services;
        }
    }
}
