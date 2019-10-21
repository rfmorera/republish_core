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
    public class LongQueueTimer : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger _logger;

        public LongQueueTimer(IServiceProvider services,
            ILogger<ShortQueueTimer> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "LongQueue Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(15), TimeSpan.FromMinutes(4));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                _logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

                using (var scope = Services.CreateScope())
                {
                    var scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<ILongQueueService>();

                    IAsyncResult asyncResult = scopedProcessingService.Process();
                    WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
                    waitHandle.WaitOne();
                }
                _logger.LogInformation(
                    "Completed - Consume Scoped Service Hosted Service is completed.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error en LongQueueTimer");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
