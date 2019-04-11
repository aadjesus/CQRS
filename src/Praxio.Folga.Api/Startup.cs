using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using BgmRodotec.Framework.Configuration.Api.Client;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NHibernate;
using Praxio.Folga.Api.Auth;
using Praxio.Folga.Api.Configurations;
using Praxio.Folga.Api.Filters;
using Praxio.Folga.Application.AutoMapper;
using Praxio.Folga.Domain.Extensions;
using Praxio.Folga.Domain.Interfaces;
using Praxio.Folga.Domain.Model;
using Praxio.Folga.Domain.Security;
using Praxio.Folga.Infra.CrossCutting.IoC;
using Praxio.Folga.Infra.Data.Mappings;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.RollingFileAlternate;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;
using ILogger = Serilog.ILogger;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace Praxio.Folga.Api
{
    /// <summary/>
    public class Startup
    {
        private const string Version = "1";
        private static string ProjectName;
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger _serilog;

        /// <summary/>
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            ProjectName = hostingEnvironment.ApplicationName;

            _serilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Async(s =>
                    s.RollingFileAlternate(
                        @"Logs/",
                        outputTemplate:
                        "[{ProcessId}] {Timestamp} [{ThreadId}] [{Level}] [{SourceContext}] [{Category}] {Message}{NewLine}{Exception}",
                        fileSizeLimitBytes: 10 * 1024 * 1024,
                        retainedFileCountLimit: 100,
                        formatProvider: CreateLoggingCulture()
                    ).MinimumLevel.Debug())
                .CreateLogger();

            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(hostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true)
                    .AddEnvironmentVariables()
                    ;

                if (hostingEnvironment.IsDevelopment())
                    SelfLog.Enable(Console.Error);

                _configuration = builder.Build();
            }
            catch (Exception ex)
            {
                _serilog?.Error("Erro no Startup: " + ex);
            }
        }

        /// <summary/>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                var policy = new AuthorizationPolicyBuilder()
                   .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                   .RequireAuthenticatedUser().Build();

                services.AddAuthorization(auth =>
                {
                    auth.AddPolicy("Bearer", policy);
                });

                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = TokenAuthOption.Key,
                        ValidAudience = TokenAuthOption.Audience,
                        ValidIssuer = TokenAuthOption.Issuer,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(0)
                    };
                });

                var connectionString = _configuration.GetConnectionString(ConfigurationKeys.KEY_DEFAULT_CONNECTION);
                if (string.IsNullOrEmpty(connectionString))
                    connectionString = new ConfigManager(_configuration[ConfigurationKeys.KEY_CONFIGURATION_API])
                        .ObterConfig(ConfigurationSetting.API_PLANTAO_CONNECTION_STRING)
                        .Result;

#if DEBUG
                Debug.WriteLine(new string('=', connectionString.Length));
                Debug.WriteLine(" " + connectionString, "CONNECTIONSTRING");
                Debug.WriteLine(new string('=', connectionString.Length));
#endif

                services.AddSingleton(ctx => _serilog);

                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

                services.AddAutoMapper(config =>
                {
                    config.ForAllMaps((map, expression) =>
                    {
                        foreach (var unmappedPropertyName in map.GetUnmappedPropertyNames())
                            expression.ForMember(unmappedPropertyName,
                                configurationExpression => configurationExpression.Ignore());
                    });

                    config.AddProfiles(typeof(DomainToViewModelMappingProfile).Assembly);
                });

                //var hack = default(UnitOfWork);

                var sessionFactory = Fluently
                    .Configure()
                    .Database(OracleDataClientConfiguration.Oracle10.ConnectionString(connectionString))
                    .ExposeConfiguration(e =>
                    {
                        e.SetProperty("connection.driver_class", "NHibernate.Driver.OracleManagedDataClientDriver, NHibernate, Version=5.1.0.0, Culture=neutral, PublicKeyToken=aa95f207798dfdb4");
                        //e.Interceptor = new NHSQLInterceptor(_serilog);
                        e.Interceptor = new NHSQLInterceptor();
                    })
                    .Mappings(m => m.FluentMappings.AddFromAssembly(typeof(MapBase<>).Assembly))
                    .BuildSessionFactory();

                RegisterSwagger(services);
                ConfigureMvc(services, policy);

                return new AutofacServiceProvider(ConfigureAutoFac(services, sessionFactory));
            }
            catch (Exception ex)
            {
                _serilog?.Error("Erro no ConfigureServices: " + ex);
            }

            return null;
        }

        /// <summary/>
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            try
            {
                loggerFactory.AddConsole();

                if (env.IsDevelopment())
                    app.UseDeveloperExceptionPage();
                else
                    app.UseHsts();

                app.UseCors(c =>
                {
                    c.AllowAnyHeader();
                    c.AllowAnyMethod();
                    c.AllowAnyOrigin();
                });

                app.UseMvcWithDefaultRoute();
                app.UseHttpsRedirection();
                app.UseMvc();

                var pathBase = _configuration["PATH_BASE"];
                app.UseSwagger();
                app.UseSwaggerUI(s =>
                {
                    s.SwaggerEndpoint($"{(string.IsNullOrEmpty(pathBase) ? string.Empty : pathBase)}/swagger/v{Version}/swagger.json", $"{ProjectName} API");
                });

            }
            catch (Exception ex)
            {
                _serilog?.Error("Erro no Configure: " + ex);
            }
        }

        private void RegisterSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.DescribeAllEnumsAsStrings();
                s.DescribeStringEnumsInCamelCase();
                s.DescribeAllParametersInCamelCase();
                s.SwaggerDoc($"v1", new Info
                {
                    Version = $"v1",
                    Title = ProjectName,
                    Description = $"{ProjectName} API Swagger surface"
                });

                foreach (var itemFile in Directory.GetFiles(PlatformServices.Default.Application.ApplicationBasePath, "*.xml"))
                    s.IncludeXmlComments(itemFile);

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string [] { } },
                };

                s.AddSecurityDefinition(
                              "Bearer",
                              new ApiKeyScheme
                              {
                                  In = "header",
                                  Description = "Copie 'Bearer ' + token'",
                                  Name = "Authorization",
                                  Type = "apiKey"
                              });

                s.AddSecurityRequirement(security);
            });
        }

        private void ConfigureMvc(IServiceCollection services, AuthorizationPolicy policy)
        {
            services
                .AddWebApi(options =>
                {
                    options.Filters.Add(new AuthorizeFilter(policy));
                    options.OutputFormatters.Remove(new XmlDataContractSerializerOutputFormatter());
                    options.Filters.Add<HttpGlobalExceptionFilter>();
                })
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    opt.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    opt.SerializerSettings.ContractResolver = new CustomContractResolver();
                })
                .AddControllersAsServices();
        }

        private IContainer ConfigureAutoFac(IServiceCollection services, ISessionFactory sessionFactory)
        {
            services.AddScoped<IUsuario, Usuario>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));

            //services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, Domain.Tasks.GravarHorarioAutomaticoTask>();
            services.AddHostedService<Domain.Tasks.ExemploBackground>();

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new ApplicationModule());

            builder.Register(f => sessionFactory).SingleInstance();

            var parametersRepository = _configuration.GetSection("Parameters").Get<ParametersAppSettings>() ?? new ParametersAppSettings() { QtdePaginacao = 5 };
            builder.Register(r => parametersRepository).As<IParametersAppSettings>();

            return builder.Build();
        }

        private CultureInfo CreateLoggingCulture()
        {
            var loggingCulture = new CultureInfo("");
            loggingCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            loggingCulture.DateTimeFormat.LongTimePattern = "HH:mm:ss.fffzz";

            return loggingCulture;
        }
    }
}
