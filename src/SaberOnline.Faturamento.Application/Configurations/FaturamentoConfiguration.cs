
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaberOnline.Application.Commands.RealizarPagamento;
using SaberOnline.Application.Events.GerarLinkPagamento;
using SaberOnline.Core;
using SaberOnline.Core.DomainHadlers;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.FaturamentoEvents;
using SaberOnline.Faturamento.Data.Contexts;
using SaberOnline.Faturamento.Data.Repositories;
using SaberOnline.Faturamento.Domain.Interfaces;
using SaberOnline.Messages.Comunications.FaturamentoCommands;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SaberOnline.Application.Application.Configurations;

[ExcludeFromCodeCoverage]
public static class FaturamentoConfiguration
{
    public static IServiceCollection ConfigurarFaturamentoApplication(this IServiceCollection services, string stringConexao, bool ehProducao)
    {
        return services
            .ConfigurarInjecoesDependenciasRepository()
            .ConfigurarInjecoesDependenciasApplication()
            .ConfigurarRepositorios(stringConexao, ehProducao);
    }

    private static IServiceCollection ConfigurarInjecoesDependenciasRepository(this IServiceCollection services)
    {
        services.AddScoped<IFaturamentoRepository, FaturamentoRepository>();
        return services;
    }

    private static IServiceCollection ConfigurarInjecoesDependenciasApplication(this IServiceCollection services)
    {
        services.AddScoped<IMediatorHandler, MediatorHandler>();

        services.AddScoped<INotificationHandler<DomainNotificacaoRaiz>, DomainNotificacaoHandler>();
        services.AddScoped<INotificationHandler<GerarLinkPagamentoEvent>, GerarLinkPagamentoEventHandler>();

        services.AddScoped<IRequestHandler<RealizarPagamentoCommand, bool>, RealizarPagamentoCommandHandler>();

        return services;
    }

    private static IServiceCollection ConfigurarRepositorios(this IServiceCollection services, string stringConexao, bool ehProducao)
    {
        services.AddDbContext<FaturamentoDbContext>(o =>
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
