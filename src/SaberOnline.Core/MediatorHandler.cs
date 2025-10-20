using MediatR;
using SaberOnline.Core.Messages;

namespace SaberOnline.Core
{
    public class MediatorHandler(IMediator mediator) : IMediatorHandler
    {
        private readonly IMediator _mediator = mediator;

        public async Task PublicarEvento<T>(T evento) where T : EventoRaiz
        {
            await _mediator.Publish(evento);
        }

        public async Task<bool> EnviarComando<T>(T comando) where T : CommandRaiz
        {
            return await _mediator.Send(comando);
        }

        public async Task PublicarNotificacaoDominio<T>(T notificacao) where T : DomainNotificacaoRaiz
        {
            await _mediator.Publish(notificacao);
        }
    }
}
