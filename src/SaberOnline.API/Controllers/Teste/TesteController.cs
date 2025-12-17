using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SaberOnline.API.Controllers.Teste
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        public TesteController()
        {
        }

        [Authorize]
        [HttpGet("teste")]
        public IActionResult Teste()
        {
            return Ok(new { autenticado = User.Identity.IsAuthenticated });
        }
    }
}
