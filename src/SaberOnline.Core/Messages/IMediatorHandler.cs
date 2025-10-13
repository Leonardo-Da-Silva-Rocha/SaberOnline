
namespace SaberOnline.Core.Messages
{
    public interface IMediatorHandler
    {
        Task PublicarEvento<T>(T evento) where T : EventoRaiz;
        Task<bool> EnviarComando<T>(T comando) where T : CommandRaiz;
        Task PublicarNotificacaoDominio<T>(T notificacao) where T : DomainNotificacaoRaiz;
    }
}
