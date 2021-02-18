﻿using DFC.App.JobProfile.Data.Contracts;
using DFC.Compui.Telemetry.HostedService;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.HostedServices
{
    public abstract class CacheLoadingBackgroundService<TLoader> :
        BackgroundService
        where TLoader : class, ILoadCacheData
    {
        private readonly ILogger<CacheLoadingBackgroundService<TLoader>> _logger;
        private readonly CmsApiClientOptions _clientOptions;
        private readonly ILoadCacheData _cacheLoader;
        private readonly IHostedServiceTelemetryWrapper _serviceTelemetryWrapper;

        protected CacheLoadingBackgroundService(
            ILogger<CacheLoadingBackgroundService<TLoader>> logger,
            CmsApiClientOptions clientOptions,
            ILoadCacheData cacheLoader,
            IHostedServiceTelemetryWrapper hostedServiceTelemetryWrapper)
        {
            _logger = logger;
            _clientOptions = clientOptions;
            _cacheLoader = cacheLoader;
            _serviceTelemetryWrapper = hostedServiceTelemetryWrapper;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cache reload started");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cache reload stopped");

            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_clientOptions.BaseAddress != null)
            {
                _logger.LogInformation("Cache reload executing");

                var task = _serviceTelemetryWrapper.Execute(() => _cacheLoader.Reload(stoppingToken), nameof(CacheLoadingBackgroundService<TLoader>));

                if (!task.IsCompletedSuccessfully)
                {
                    _logger.LogInformation("Cache reload didn't complete successfully");
                    if (task.Exception != null)
                    {
                        _logger.LogError(task.Exception.ToString());
                        throw task.Exception;
                    }
                }

                return task;
            }

            return Task.CompletedTask;
        }
    }
}
