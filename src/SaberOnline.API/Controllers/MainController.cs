using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SaberOnline.API.Autentications;
using SaberOnline.API.Enumerators;
using SaberOnline.Core.DomainHadlers;
using SaberOnline.Core.Exceptions;
using SaberOnline.Core.Messages;
using System.Net;

namespace SaberOnline.API.Controllers
{

    [ApiController]
    public class MainController(IAppIdentityUser appIdentityUser,
    INotificationHandler<DomainNotificacaoRaiz> notifications,
    IMediatorHandler mediatorHandler) : ControllerBase
    {
        private readonly IAppIdentityUser _appIdentityUser = appIdentityUser;
        protected readonly DomainNotificacaoHandler _notifications = (DomainNotificacaoHandler)notifications;
        protected readonly IMediatorHandler _mediatorHandler = mediatorHandler;

        protected bool OperacaoValida() => !_notifications.TemNotificacao();
        public Guid UserId => _appIdentityUser.ObterUsuarioId();
        public bool EstahAutenticado => _appIdentityUser.EstahAutenticado();
        public string Email => _appIdentityUser.ObterEmail();
        public bool EhAdministrador => _appIdentityUser.EhAdministrador();

        protected ActionResult GenerateDomainExceptionResponse(object? result = null,
            ResponseTypeEnum responseType = ResponseTypeEnum.Success,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            DomainException exception = null)
        {
            List<string> errors = [];
            if (exception != null)
            {
                errors.AddRange(exception.Errors ?? [exception.Message]);
            }

            return GenerateResponse(result, responseType, statusCode, errors);
        }

        protected ActionResult GenerateResponse(object? result = null,
            ResponseTypeEnum responseType = ResponseTypeEnum.Success,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            IList<string> errors = null)
        {
            if (OperacaoValida() && ((int)statusCode >= 200 && (int)statusCode <= 299))
            {
                return new JsonResult(new
                {
                    success = true,
                    type = responseType.ToString(),
                    result
                })
                {
                    StatusCode = (int)statusCode
                };
            }

            errors ??= [];
            if (_notifications.TemNotificacao())
            {
                var notificationErrors = _notifications.ObterNotificacoes().Select(n => $"({n.Chave}: {n.RaizAgregacao}) Mensagem: {n.Valor}").ToList();
                foreach (string erro in notificationErrors)
                {
                    errors.Add(erro);
                }
            }

            return new JsonResult(new
            {
                success = false,
                type = responseType.ToString(),
                errors
            })
            {
                StatusCode = (int)statusCode
            };
        }

        protected ActionResult GenerateModelStateResponse(ResponseTypeEnum responseType, HttpStatusCode statusCode, ModelStateDictionary modelState)
        {
            return new JsonResult(new
            {
                success = false,
                type = responseType.ToString(),
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            })
            {
                StatusCode = (int)statusCode
            };
        }
    
    }
}
