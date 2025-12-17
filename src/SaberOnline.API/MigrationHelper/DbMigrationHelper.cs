using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaberOnline.Aluno.Data.Context;
using SaberOnline.Autenticacao.Data;
using SaberOnline.Conteudo.Data;
using SaberOnline.Conteudo.Domain.Entities;
using SaberOnline.Conteudo.Domain.ValueObjects;

namespace SaberOnline.API.MigrationHelper
{
    public static class DbMigrationHelper
    {
        private static AutenticacaoDbContext _identityContext = null;
        private static ConteudoContext _conteudoContext = null;
        private static AlunoDbContext _alunoContext = null;
        
        private static UserManager<IdentityUser> _userManager = null;

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
            _conteudoContext = scope.ServiceProvider.GetRequiredService<ConteudoContext>();
            _alunoContext = scope.ServiceProvider.GetRequiredService<AlunoDbContext>();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (env.IsDevelopment())
            {
                await _identityContext.Database.MigrateAsync();
                await _conteudoContext.Database.MigrateAsync();
                await _alunoContext.Database.MigrateAsync();
                await PopularDatabaseAsync();
            }
        }

        private static async Task PopularDatabaseAsync()
        {
            if (_identityContext.Roles.Any()) { return; }

            string roleAdminId = await CriarRegraAcessoAsync(_identityContext, "Administrador");
            string roleUsuarioId = await CriarRegraAcessoAsync(_identityContext, "Usuario");

            await CriarUsuarioAsync("teste@gmail.com", "Password@2025", "teste filho", new DateTime(1999, 09, 08), roleAdminId, true);
            await CriarUsuarioAsync("antonio@gmail.com", "Password@2025", "antonio fabio", new DateTime(1998, 12, 31), roleUsuarioId, false);
            await CriarUsuarioAsync("maico@gmail.com", "Password@2025", "maico silva", new DateTime(2000, 06, 07), roleUsuarioId, false);
        }

        private static async Task<string> CriarRegraAcessoAsync(AutenticacaoDbContext identityContext, string role)
        {
            string roleId = Guid.NewGuid().ToString();
            identityContext.Roles.Add(new IdentityRole
            {
                Id = roleId,
                Name = role,
                NormalizedName = role,
                ConcurrencyStamp = DateTime.Now.ToString()
            });

            await identityContext.SaveChangesAsync();

            return roleId;
        }

        private static async Task CriarUsuarioAsync(string email, string senha, string nome, DateTime dataNascimento, string roleId, bool ehAdmin)
        {
            var identityUser = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(identityUser, senha);

            if (result.Succeeded)
            {
                #region Roles
                _identityContext.UserRoles.Add(new IdentityUserRole<string>()
                {
                    RoleId = roleId,
                    UserId = identityUser.Id.ToString()
                });

                await _identityContext.SaveChangesAsync();
                #endregion Roles

                #region Data
                Guid userId = Guid.Parse(identityUser.Id);
                if (ehAdmin)
                {
                    await CriarCursoAsync();
                }
                else
                {
                    await CriarAlunoAsync(Guid.Parse(identityUser.Id), nome, email, dataNascimento);
                }
                #endregion
            }
        }

        private static async Task CriarCursoAsync()
        {
            ConteudoProgramatico conteudoCurso1 = new ConteudoProgramatico("Aprender a criar sites e sistemas modernos com .NET", "Durante o curso, você vai ver como montar uma aplicação completa: backend em .NET e frontend com Angular.");

            Curso curso1 = new Curso("Curso de Desenvolvimento Full Stack", 3500m, DateTime.Today.AddYears(2), conteudoCurso1);
            curso1.AdicionarAula("1 - Fundamentos do .NET", 1, 1, "https://curso.com/aula1");
            curso1.AdicionarAula("2 - Criando APIs REST com ASP.NET Core", 2, 2, "https://curso.com/aula2");

            ConteudoProgramatico conteudoCurso2 = new ConteudoProgramatico("Aprender na prática como gerenciar projetos usando métodos ágeis como Scrum e Kanban", "Durante o curso, você vai entender como montar, organizar e tocar times ágeis, entregando valor de forma contínua com frameworks ágeis.");

            Curso curso2 = new Curso(
                "Gestão Ágil de Projetos com Scrum e Kanban",
                2800m,
                DateTime.Today.AddYears(2),
                conteudoCurso2
            );

            curso2.AdicionarAula("1 - O que é o Manifesto Ágil e seus princípios", 1, 1, "https://curso.com/aula1");
            curso2.AdicionarAula("2 - Como funciona o Scrum na prática", 2, 2, "https://curso.com/aula2");



            ConteudoProgramatico conteudoCurso3 = new ConteudoProgramatico("Preparar você para o mercado de análise de dados usando as ferramentas mais atuais", "Durante o curso, você vai aprender desde como modelar dados até criar dashboards e análises de performance com Power BI.");

            Curso curso3 = new Curso(
                "Análise de Dados com Power BI e SQL Server",
                3200m,
                DateTime.Today.AddYears(2),
                conteudoCurso3
            );

            curso3.AdicionarAula("1 - Introdução à Análise de Dados", 1, 1, "https://curso.com/aula1");
            curso3.AdicionarAula("2 - Fundamentos de SQL Server para análise", 3, 2, "https://curso.com/aula2");


            await _conteudoContext.Cursos.AddAsync(curso1);
            await _conteudoContext.Cursos.AddAsync(curso2);
            await _conteudoContext.Cursos.AddAsync(curso3);
            await _conteudoContext.SaveChangesAsync();
        }

        private static async Task CriarAlunoAsync(Guid identityId, string nome, string email, DateTime dataNascimento)
        {
            var listaCursos = _conteudoContext.Cursos.ToList();
            var listaAulas = _conteudoContext.Aulas.ToList();

            Aluno.Domain.Entities.Aluno aluno = new Aluno.Domain.Entities.Aluno(nome, email, dataNascimento);
            aluno.IdentificarCodigoUsuarioNoSistema(identityId);

            var finalizarCurso = true;
            foreach (var curso in listaCursos)
            {
                aluno.MatricularEmCurso(curso.Id, curso.Nome, curso.Valor);
                var matriculaCurso = aluno.MatriculasCursos.Last();
                aluno.AtualizarPagamentoMatricula(matriculaCurso.Id);

                foreach (var aula in listaAulas.Where(x => x.CursoId == curso.Id).ToList())
                {
                    aluno.RegistrarHistoricoAprendizado(matriculaCurso.Id, aula.Id, aula.Descricao, DateTime.Today);
                }

                if (finalizarCurso)
                {
                    finalizarCurso = false;
                    aluno.ConcluirCurso(matriculaCurso.Id);
                    aluno.RequisitarCertificadoConclusao(matriculaCurso.Id, $"/var/tmp/alunos/{aluno.Id}/certificados/{curso.Id}.pdf");
                }
            }

            _alunoContext.Alunos.Add(aluno);
            await _alunoContext.SaveChangesAsync();
        }
    }
}