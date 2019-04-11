using Autofac;
using FluentValidation;
using MediatR;
using Praxio.Folga.Domain.Commands;
using Praxio.Folga.Domain.Pipelines;
using System.Collections.Generic;
using Module = Autofac.Module;

namespace Praxio.Folga.Infra.CrossCutting.IoC
{
    public class MediatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly)
                .AsImplementedInterfaces().InstancePerLifetimeScope()
                ;

            var messageTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(IRequestHandler<>),
                typeof(INotificationHandler<>)
            };

            foreach (var messageType in messageTypes)
                builder
                    .RegisterAssemblyTypes(typeof(CommandHandler).Assembly)
                    .AsClosedTypesOf(messageType)
                    .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder
                .RegisterAssemblyTypes(typeof(CommandHandler).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            builder.Register<SingleInstanceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t =>
                {
                    object o;
                    return componentContext.TryResolve(t, out o) ? o : null;
                };
            });

            builder.Register<MultiInstanceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();

                return t =>
                {
                    var resolved =
                        (IEnumerable<object>)componentContext.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
                    return resolved;
                };
            });

            builder.RegisterGeneric(typeof(ValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}