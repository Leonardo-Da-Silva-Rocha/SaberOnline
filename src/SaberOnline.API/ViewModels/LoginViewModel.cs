using System.ComponentModel.DataAnnotations;

namespace SaberOnline.API.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Atributo {0} é obrigatório")]
        [EmailAddress(ErrorMessage = "Atributo {0} está em formato inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Atributo {0} é obrigatório")]
        [StringLength(maximumLength: 25, ErrorMessage = "Atributo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 8)]
        public string Password { get; set; }
    }
}
