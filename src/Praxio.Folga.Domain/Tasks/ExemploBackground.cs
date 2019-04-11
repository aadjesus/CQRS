using BgmRodotec.Framework.Domain.Core.Bus;
using Microsoft.Extensions.Hosting;
using Praxio.Folga.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Praxio.Folga.Domain.Tasks
{
    public class ExemploBackground : BackgroundService
    {
        private readonly IMediatorHandler _mediator;
        private readonly Serilog.ILogger _serilog;
        private readonly IParametersAppSettings _parametersAppSettings;

        public ExemploBackground(
            IMediatorHandler mediator,
            Serilog.ILogger serilog,
            IParametersAppSettings parametersAppSettings)
        {
            _mediator = mediator;
            _serilog = serilog;
            _parametersAppSettings = parametersAppSettings;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_parametersAppSettings.GPS)
                return StopAsync(stoppingToken);

            Console.Write("Exemplo....");

            return Task.CompletedTask;
        }
    }
}
