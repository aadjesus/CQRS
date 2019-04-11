using BgmRodotec.Framework.Domain.Core.Bus;
using BgmRodotec.Framework.Domain.Core.Commands;
using BgmRodotec.Framework.Domain.Core.Events;
using BgmRodotec.Framework.Domain.Core.Notifications;
using MediatR;
using System.Threading.Tasks;

namespace Praxio.Folga.Infra.CrossCutting.Bus
{
    public sealed class InMemoryBus : IMediatorHandler
    {
        private readonly IMediator _mediator;

        public InMemoryBus(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public Task<TResult> SendCommand<T, TResult>(T command) where T : Command
        {
            var cmd = (IRequest<TResult>)command;
            return _mediator.Send(cmd);
        }

        public Task RaiseEvent<T>(T @event) where T : Event
        {
            return _mediator.Publish(@event);
        }

        public Task Notify<T>(T notification) where T : Notification
        {
            return _mediator.Publish(notification);
        }

        public Task Send<T>(T command) where T : ICommand
        {
            return _mediator.Send(command);
        }

        public Task<TResult> Send<T, TResult>(T command) where T : ICommand
        {
            var cmd = (IRequest<TResult>)command;
            return _mediator.Send(cmd);
        }
    }
}