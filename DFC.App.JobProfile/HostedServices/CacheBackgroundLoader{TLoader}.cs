using DFC.App.JobProfile.Cacheing;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.Compui.Telemetry.HostedService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.HostedServices
{
    public abstract class CacheBackgroundLoader<TLoader> :
        BackgroundService
        where TLoader : class, ILoadCacheData
    {
        private readonly ILogger<CacheBackgroundLoader<TLoader>> _logger;
        private readonly ContentApiOptions _clientOptions;
        private readonly ILoadCacheData _cacheLoader;
        private readonly IHostedServiceTelemetryWrapper _serviceTelemetryWrapper;

        protected CacheBackgroundLoader(
            ILogger<CacheBackgroundLoader<TLoader>> logger,
            ContentApiOptions clientOptions,
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

                var task = _serviceTelemetryWrapper.Execute(() => _cacheLoader.Reload(stoppingToken), nameof(CacheBackgroundLoader<TLoader>));

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
