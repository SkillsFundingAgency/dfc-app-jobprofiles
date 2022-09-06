using DFC.App.JobProfile.Data.Contracts;
using DFC.Compui.Telemetry.HostedService;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.HostedServices
{
    [ExcludeFromCodeCoverage]
    public class SharedContentReloadBackgroundService : BackgroundService
    {
        private readonly ILogger<SharedContentReloadBackgroundService> logger;
        private readonly CmsApiClientOptions cmsApiClientOptions;
        private readonly ISharedContentReloadService staticContentReloadService;
        private readonly IHostedServiceTelemetryWrapper hostedServiceTelemetryWrapper;

        public SharedContentReloadBackgroundService(ILogger<SharedContentReloadBackgroundService> logger, CmsApiClientOptions cmsApiClientOptions, ISharedContentReloadService staticContentReloadService, IHostedServiceTelemetryWrapper hostedServiceTelemetryWrapper)
        {
            this.logger = logger;
            this.cmsApiClientOptions = cmsApiClientOptions;
            this.staticContentReloadService = staticContentReloadService;
            this.hostedServiceTelemetryWrapper = hostedServiceTelemetryWrapper;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Static content reload started");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Static content reload stopped");

            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (cmsApiClientOptions.BaseAddress != null)
            {
                logger.LogInformation("Static content reload executing");

                var task = hostedServiceTelemetryWrapper.Execute(() => staticContentReloadService.Reload(stoppingToken), nameof(SharedContentReloadBackgroundService));

                if (!task.IsCompletedSuccessfully)
                {
                    logger.LogInformation("Static content reload didn't complete successfully");
                    if (task.Exception != null)
                    {
                        logger.LogError(task.Exception.ToString());
                        throw task.Exception;
                    }
                }

                return task;
            }

            return Task.CompletedTask;
        }
    }
}
