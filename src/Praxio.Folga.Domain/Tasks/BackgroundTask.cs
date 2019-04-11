using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Praxio.Folga.Domain.Tasks
{

    /// <summary/>
    public abstract class BackgroundTask : IHostedService, IDisposable
    {
        private Task _executingTask;

        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        /// <summary/>
        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        /// <summary/>
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            return _executingTask.IsCompleted
                ? _executingTask
                : Task.CompletedTask;
        }

        /// <summary/>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }

        }

        /// <summary/>
        public virtual void Dispose() =>
            _stoppingCts.Cancel();
    }
}