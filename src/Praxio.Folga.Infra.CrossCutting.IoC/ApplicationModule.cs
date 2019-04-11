using Autofac;
using Praxio.Folga.Application.Services;
using Praxio.Folga.Domain.Interfaces;
using Praxio.Folga.Infra.CrossCutting.Bus;
using Praxio.Folga.Infra.Data.Repository;
using Praxio.Folga.Infra.Data.UoW;
using BgmRodotec.Framework.Domain.Core.Bus;
using Praxio.Folga.Domain.Auth;

namespace Praxio.Folga.Infra.CrossCutting.IoC
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(typeof(AppService).Assembly)
                .AsImplementedInterfaces().InstancePerLifetimeScope()
                //.PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                ;

            builder
                .RegisterAssemblyTypes(typeof(Repository<>).Assembly)
                .AsImplementedInterfaces().InstancePerLifetimeScope()
                //.WithProperty("QtdePaginacao", 10)
                //.PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                ;

            builder.RegisterType<InMemoryBus>().As<IMediatorHandler>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<UsuarioBase>().As<IUsuario>().InstancePerLifetimeScope();
        }
    }
}