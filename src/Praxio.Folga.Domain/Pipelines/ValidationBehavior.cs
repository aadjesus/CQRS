using BgmRodotec.Framework.Domain.Core.Bus;
using BgmRodotec.Framework.Domain.Core.Commands;
using FluentValidation;
using MediatR;
using Praxio.Folga.Domain.Notifications;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Praxio.Folga.Domain.Pipelines
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : Command
        where TResponse : CommandResponse
    {
        private readonly IMediatorHandler _bus;
        private readonly IValidator<TRequest>[] _validators;

        public ValidationBehavior(IMediatorHandler bus, IValidator<TRequest>[] validators)
        {
            _bus = bus;
            _validators = validators;
        }


        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                foreach (var validationFailure in failures)
                    await _bus.Notify(new DomainNotification(request.MessageType, validationFailure.ErrorMessage));
                return (TResponse)CommandResponse.Fail;
            }

            var response = await next();
            return response;
        }
    }
}