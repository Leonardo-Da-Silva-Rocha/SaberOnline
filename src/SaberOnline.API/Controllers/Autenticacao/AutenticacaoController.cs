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

            var registerResult = await _userManager.CreateAsync(identitiyUser, registroViewModel.Password);

            if (registerResult.Succeeded)
            {
                try
                {
                    
                    IdentityRole role = null;
                    if (registroViewModel.EhAdministrador)
                    {
                        role = _identityContext.Roles.FirstOrDefault(x => x.Name == "Administrador");
                    }
                    else
                    {
                        role = _identityContext.Roles.FirstOrDefault(x => x.Name == "Usuario");
                    }

                    if (role != null)
                    {
                        _identityContext.UserRoles.Add(new IdentityUserRole<string>()
                        {
                            RoleId = role.Id,
                            UserId = identitiyUser.Id
                        });

                        await _identityContext.SaveChangesAsync();
                    }

                    var sucesso = true;

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
            if (user == null) { user = await _userManager.FindByEmailAsync(email); }
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (ClaimTypes.Name, user.UserName),
            new (ClaimTypes.NameIdentifier, user.Id.ToString())
        };

            if (roles.Any(x => x == "Administrador"))
            {
                claims.Add(new Claim("nivel", "Admin"));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSettings.Secret);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _appSettings.JwtSettings.Issuer,
                Audience = _appSettings.JwtSettings.Audience,
                Expires = DateTime.UtcNow.AddHours(_appSettings.JwtSettings.ExpirationInHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);

            return encodedToken;
        }
    }
}