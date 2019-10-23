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
    public class TemporizadoresTimer : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger _logger;

        public TemporizadoresTimer(IServiceProvider services,
            ILogger<TemporizadoresTimer> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Check Temporizadores  is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                _logger.LogInformation(
                "Check Temporizadores is working.");

                using (var scope = Services.CreateScope())
                {
                    var scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<IChequerService>();

                    IAsyncResult asyncResult = scopedProcessingService.CheckAllTemporizadores();
                    WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
                    waitHandle.WaitOne();
                }
                _logger.LogInformation(
                    "Completed - Check Temporizadores  is completed.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error en TemporizadoresTimer");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Check Temporizadores is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
