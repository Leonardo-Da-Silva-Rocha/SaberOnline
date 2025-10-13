using SaberOnline.API.Configurations;
using SaberOnline.API.Settings;
using SaberOnline.Core;
using SaberOnline.Core.Messages;
using SaberOnline.Aluno.Application.Configurations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

builder.Services.AddHttpContextAccessor()
    
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    Assembly.GetExecutingAssembly(),
    typeof(DomainNotificacaoRaiz).Assembly
))
    .AddScoped<IMediatorHandler, MediatorHandler>()
    .ConfigurarJwt(appSettings.JwtSettings)
    .ConfigurarAutenticacao(appSettings.DatabaseSettings, builder.Environment.IsProduction())
    .ConfigurarAlunoApplication(appSettings.DatabaseSettings.ConnectionStringAluno, builder.Environment.IsProduction())
    .ConfigurarApi()
    .ConfigurarSwagger();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.ExecutarConfiguracaoAmbiente();
app.Run();