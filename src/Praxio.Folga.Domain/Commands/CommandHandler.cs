using BgmRodotec.Framework.Domain.Core.Bus;
using BgmRodotec.Framework.Domain.Core.Commands;
using MediatR;
using Praxio.Folga.Domain.Interfaces;
using Praxio.Folga.Domain.NotificationHandlers;
using Praxio.Folga.Domain.Notifications;
using Praxio.Folga.Domain.Resources;
using System.Threading.Tasks;

namespace Praxio.Folga.Domain.Commands
{
    /// <summary/>
    public class CommandHandler
    {
        protected readonly IMediatorHandler _mediator;
        protected readonly DomainNotificationHandler _notifications;
        protected readonly IUnitOfWork _uow;

        /// <summary/>
        protected CommandHandler(
            IUnitOfWork uow,
            IMediatorHandler mediator,
            INotificationHandler<DomainNotification> notifications)
        {
            _uow = uow;
            _notifications = (DomainNotificationHandler)notifications;
            _mediator = mediator;
        }

        /// <summary/>
        protected bool Commit()
        {
            if (_notifications.HasNotifications())
            {
                _uow.Rollback();
                return false;
            }

            var commandResponse = _uow.Commit();
            if (commandResponse.Success)
                return true;

            _mediator.Notify(new DomainNotification("Commit", Mensagens.ErroCommit));
            return false;
        }

        /// <summary/>
        public Task<CommandResponse> ExecutarCommitOuRollBack(string mensagem)
        {
            if (!Commit())
                return Task.FromResult(CommandResponse.Fail);

            _mediator.Notify(new DomainDataNotification()
            {
                Data = new
                {
                    Resultado = mensagem
                }
            });

            return Task.FromResult(CommandResponse.Ok);
        }
    }
}
