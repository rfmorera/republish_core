using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.BackgroundTasks
{
    public class CleanerTemporizador : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger _logger;

        public CleanerTemporizador(IServiceProvider services,
            ILogger<CleanerTemporizador> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Cleaner Temporizadores  is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                _logger.LogInformation(
                "Cleaner Temporizadores is working.");

                using (var scope = Services.CreateScope())
                {
                    var scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<IChequerService>();

                    IAsyncResult asyncResult = scopedProcessingService.ResetRemoveQueue();
                    WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
                    waitHandle.WaitOne();
                }
                _logger.LogInformation(
                    "Completed - Cleaner Temporizadores  is completed.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error en TemporizadoresTimer");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Cleaner Temporizadores is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
