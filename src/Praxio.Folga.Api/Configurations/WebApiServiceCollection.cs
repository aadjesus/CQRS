using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Praxio.Folga.Api.Configurations
{
    /// <summary/>
    public static class WebApiServiceCollectionExtensions
    {
        /// <summary/>
        public static IMvcBuilder AddWebApi(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var builder = services.AddMvcCore();
            builder.AddJsonFormatters();
            builder.AddApiExplorer();
            builder.AddCors();

            return new MvcBuilder(builder.Services, builder.PartManager);
        }

        /// <summary/>
        public static IMvcBuilder AddWebApi(this IServiceCollection services, Action<MvcOptions> setupAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            var builder = services.AddWebApi();
            builder.Services.Configure(setupAction);

            return builder;
        }
    }
}