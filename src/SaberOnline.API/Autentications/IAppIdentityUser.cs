namespace SaberOnline.API.Autentications
{
    public interface IAppIdentityUser
    {
        Guid ObterUsuarioId();
        bool EstahAutenticado();
        bool EhAdministrador();
        string ObterEmail();
    }
}
