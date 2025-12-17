using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SaberOnline.API.Autentications;
using SaberOnline.API.Settings;
using SaberOnline.API.ViewModels;
using SaberOnline.Autenticacao.Data;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoCommands;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SaberOnline.API.Controllers.Autenticacao
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        private readonly AutenticacaoDbContext _identityContext;

        public AutenticacaoController(ILogger<AutenticacaoController> logger,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IOptions<AppSettings> appSettings,
        AutenticacaoDbContext identityContext,
        IAppIdentityUser appIdentityUser,
        INotificationHandler<DomainNotificacaoRaiz> notifications,
        IMediatorHandler mediatorHandler) : base(appIdentityUser, notifications, mediatorHandler)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _identityContext = identityContext;
        }

        [HttpPost("registro")]
        [ProducesResponseType(typeof(AutenticacaoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RegistroAsync(CadastroViewModel registroViewModel)
        {
            if (!ModelState.IsValid) return GenerateResponse(ModelState);

            var identitiyUser = new IdentityUser
            {
                UserName = registroViewModel.Email,
                Email = registroViewModel.Email,
                EmailConfirmed = true
            };

            var claimsToAdd = new List<Claim>();

            if (registroViewModel.EhAdministrador)
            {
                claimsToAdd = AdicionaClaimsAdmin();
            }
            else
            {
                claimsToAdd = AdicionaClaimsAluno();
            }

            var registerResult = await _userManager.CreateAsync(identitiyUser, registroViewModel.Password);

            if (registerResult.Succeeded)
            {
                foreach (var claim in claimsToAdd)
                {
                    await _userManager.AddClaimAsync(identitiyUser, claim);
                }
            }

            var sucesso = true;

            if (registerResult.Succeeded)
            {
                try
                {

                    if (!registroViewModel.EhAdministrador)
                    {
                        var comando = new CadastrarAlunoCommand(Guid.Parse(identitiyUser.Id), registroViewModel.Nome, registroViewModel.Email, registroViewModel.DataNascimento);
                        sucesso = await _mediatorHandler.EnviarComando(comando);
                    }

                    if (sucesso)
                    {
                        var loginOutput = new AutenticacaoViewModel
                        {
                            Id = Guid.Parse(identitiyUser.Id),
                            Nome = registroViewModel.Nome,
                            Email = registroViewModel.Email,
                            AccessToken = await GenerateJwt(registroViewModel.Email)
                        };

                        await _signInManager.SignInAsync(identitiyUser, false);

                        return GenerateResponse(loginOutput);
                    }
                    else
                    {
                        await _userManager.DeleteAsync(identitiyUser);
                    }
                }
                catch (Exception ex)
                {
                    await _userManager.DeleteAsync(identitiyUser);

                    _logger.LogError(ex, $"Erro ao tentar criar o usuário na base de dados: {ex.Message}");
                }
            }
            else
            {
                return GenerateResponse(registroViewModel,
                    Enumerators.ResponseTypeEnum.ValidationError,
                    System.Net.HttpStatusCode.BadRequest,
                    registerResult.Errors.Select(x => $"{x.Code}-{x.Description}").ToList());
            }

            return GenerateResponse();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(AutenticacaoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> LoginAsync(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return GenerateResponse(ModelState);

            var loginResult = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, false, true);

            if (loginResult.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

                var loginOutput = new AutenticacaoViewModel
                {
                    Id = Guid.Parse(user.Id),
                    Nome = user.UserName,
                    Email = user.Email,
                    AccessToken = await GenerateJwt(loginViewModel.Email, user)
                };

                return GenerateResponse(loginOutput);
            }
            else
            {
                return GenerateResponse(loginViewModel,
                    Enumerators.ResponseTypeEnum.ValidationError,
                    System.Net.HttpStatusCode.BadRequest,
                    ["Login não realizado. Verifique suas credenciais de acesso"]);
            }
        }

        private async Task<string> GenerateJwt(string email, IdentityUser user = null)
        {
            // Garante que o usuário foi carregado
            if (user == null)
                user = await _userManager.FindByEmailAsync(email);

            // 🔹 Busca todas as claims do usuário no banco (AspNetUserClaims)
            var userClaims = await _userManager.GetClaimsAsync(user);

            // 🔹 Claims padrão do token
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID único do token
            };

            // 🔹 Adiciona as claims personalizadas do usuário
            claims.AddRange(userClaims);

            // 🔹 Criação do token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _appSettings.JwtSettings.Issuer,
                Audience = _appSettings.JwtSettings.Audience,
                Expires = DateTime.UtcNow.AddHours(_appSettings.JwtSettings.ExpirationInHours),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private List<Claim> AdicionaClaimsAdmin()
        {
            var cursos = new[] { "AD", "AT", "VI", "DS" };
            var aulas = new[] { "AD", "AT", "RM" };

            var claims = new List<Claim>();

            foreach (var curso in cursos)
            {
                claims.Add(new Claim("Cursos", curso));
            }

            foreach (var aula in aulas)
            {
                claims.Add(new Claim("Aulas", aula));
            }

            claims.Add(new Claim("Admin", "GT"));

            return claims;
        }

        private List<Claim> AdicionaClaimsAluno()
        {
            var claimsToAdd = new[]
           {
                new Claim("Alunos", "MT"), // matricular
                new Claim("Alunos", "RH"), // REGISTRAR HISTORICO
                new Claim("Alunos", "CC"), //CONCLUIR CURSO
                new Claim("Alunos", "SC"), //SOLICITAR CERTIFICADO
                new Claim("Alunos", "PG"), //PAGAMENTO
                new Claim("Alunos", "GT"), //BUSCAR INFORMAÇÕES
                
            };
            return claimsToAdd.ToList();
        }

    }
}