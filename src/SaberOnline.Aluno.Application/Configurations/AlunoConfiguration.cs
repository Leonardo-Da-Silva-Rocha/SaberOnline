using SaberOnline.Core.Messages.AlunoCommands;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaberOnline.Aluno.Application.Commands.CadastrarAluno;
using SaberOnline.Aluno.Application.Commands.ConcluirCurso;
using SaberOnline.Aluno.Application.Commands.MatricularAluno;
using SaberOnline.Aluno.Application.Commands.RegistrarHistoricoAprendizado;
using SaberOnline.Aluno.Application.Commands.SolicitarCertificado;
using SaberOnline.Aluno.Application.Interfaces;
using SaberOnline.Aluno.Application.PagamentoConfirmado;
using SaberOnline.Aluno.Application.PagamentoRecusado;
using SaberOnline.Aluno.Application.ProblemaRegistroHistoricoAprendizado;
using SaberOnline.Aluno.Data.Context;
using SaberOnline.Aluno.Data.Repositories;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Core;
using SaberOnline.Core.DomainHadlers;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoEvents;
using SaberOnline.Core.Messages.FaturamentoEvents;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SaberOnline.Core.Messages.Comunications.AlunoCommands;
using SaberOnline.Aluno.Application.Queries;

namespace SaberOnline.Aluno.Application.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class AlunoConfiguration
    {
        public static IServiceCollection ConfigurarAlunoApplication(this IServiceCollection services, string stringConexao, bool ehProducao)
        {
            return services
                .ConfigurarInjecoesDependenciasRepository()
                .ConfigurarInjecoesDependenciasApplication()
                .ConfigurarRepositorios(stringConexao, ehProducao);
        }

        private static IServiceCollection ConfigurarInjecoesDependenciasRepository(this IServiceCollection services)
        {
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            return services;
        }

        private static IServiceCollection ConfigurarInjecoesDependenciasApplication(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            services.AddScoped<INotificationHandler<DomainNotificacaoRaiz>, DomainNotificacaoHandler>();

            
            services.AddScoped<INotificationHandler<PagamentoConfirmadoEvent>, PagamentoConfirmadoEventHandler>();
            services.AddScoped<INotificationHandler<PagamentoRecusadoEvent>, PagamentoRecusadoEventHandler>();
            services.AddScoped<INotificationHandler<RegistrarProblemaHistoricoAprendizadoEvent>, RegistrarProblemaHistoricoAprendizadoEventHandler>();


            services.AddScoped<IRequestHandler<CadastrarAlunoCommand, bool>, CadastrarAlunoCommandHandler>();
            services.AddScoped<IRequestHandler<ConcluirCursoCommand, bool>, ConcluirCursoCommandHandler>();
            services.AddScoped<IRequestHandler<MatricularAlunoCommand, bool>, MatricularAlunoCommandHandler>();
            services.AddScoped<IRequestHandler<RegistrarHistoricoAprendizadoCommand, bool>, RegistrarHistoricoAprendizadoCommandHandler>();
            services.AddScoped<IRequestHandler<SolicitarCertificadoCommand, bool>, SolicitarCertificadoCommandHandler>();
            
            services.AddScoped<IAlunoQueryService, AlunoQueryService>();


            return services;
        }

        private static IServiceCollection ConfigurarRepositorios(this IServiceCollection services, string stringConexao, bool ehProducao)
        {
            services.AddDbContext<AlunoDbContext>(o =>
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
