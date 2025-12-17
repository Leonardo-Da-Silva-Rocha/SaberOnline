using SaberOnline.API.Configurations;
using SaberOnline.API.Settings;
using SaberOnline.Core;
using SaberOnline.Core.Messages;
using SaberOnline.Aluno.Application.Configurations;
using SaberOnline.Conteudo.Application.Configurations;
using SaberOnline.Faturamento.Application.Configurations;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

builder.Services.AddHttpContextAccessor()
    .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())

    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    Assembly.GetExecutingAssembly(),
    typeof(DomainNotificacaoRaiz).Assembly
))
    .AddScoped<IMediatorHandler, MediatorHandler>()

    .ConfigurarJwt(appSettings.JwtSettings)
    .ConfigurarAutenticacao(appSettings.DatabaseSettings, builder.Environment.IsProduction())
    .ConfigurarAlunoApplication(appSettings.DatabaseSettings.ConnectionStringAluno, builder.Environment.IsProduction())
    .ConfigurarConteudoApplication(appSettings.DatabaseSettings.ConnectionStringConteudo, builder.Environment.IsProduction())
    .ConfigurarFaturamentoApplication(appSettings.DatabaseSettings.ConnectionStringFaturamento, builder.Environment.IsProduction())
    .ConfigurarApi()
    .ConfigurarCors()
    .AddSwaggerConfig();

var app = builder.Build();
app.ExecutarConfiguracaoAmbiente();
app.Run();