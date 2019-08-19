﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.BackgroundTasks
{
    public class DisableTimer : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger _logger;
        public IServiceProvider Services { get; }

        public DisableTimer(IServiceProvider services,
            ILogger<TemporizadoresTimer> logger)
        {
            Services = services;
            _logger = logger;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "DisabledTimer Service Hosted Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(45), TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IUserControlService>();

                IAsyncResult asyncResult = scopedProcessingService.CheckOutCeroBalanceAccount();
                WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
                waitHandle.WaitOne();
            }
            _logger.LogInformation(
                "Completed - Consume Scoped Service Hosted Service is completed.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "DisabledTimer Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
