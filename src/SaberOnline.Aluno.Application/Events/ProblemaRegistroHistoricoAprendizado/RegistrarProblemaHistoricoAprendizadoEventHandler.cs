using MediatR;
using SaberOnline.Core.Messages;
using SaberOnline.Core.Messages.AlunoEvents;

namespace SaberOnline.Aluno.Application.ProblemaRegistroHistoricoAprendizado;
public class RegistrarProblemaHistoricoAprendizadoEventHandler(IMediatorHandler mediatorHandler) : INotificationHandler<RegistrarProblemaHistoricoAprendizadoEvent>
{
    private readonly IMediatorHandler _mediatorHandler = mediatorHandler;
    private Guid _raizAgregacao;

    public async Task Handle(RegistrarProblemaHistoricoAprendizadoEvent notification, CancellationToken cancellationToken)
    {
        _raizAgregacao = notification.RaizAgregacao;
        if (!ValidarRequisicao(notification)) { return; }
    }

    private bool ValidarRequisicao(RegistrarProblemaHistoricoAprendizadoEvent notification)
    {
        notification.DefinirValidacao(new RegistrarProblemaHistoricoAprendizadoEventValidator().Validate(notification));
        if (!notification.EhValido())
        {
            foreach (var erro in notification.Erros)
            {
                _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Domain.Entities.Aluno), erro)).GetAwaiter().GetResult();
            }
            return false;
        }

        return true;
    }
}