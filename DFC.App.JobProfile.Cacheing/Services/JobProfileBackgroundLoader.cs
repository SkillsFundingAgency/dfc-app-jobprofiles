using DFC.App.JobProfile.ContentAPI.Configuration;
using DFC.Compui.Telemetry.HostedService;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.Cacheing.Services
{
    internal sealed class JobProfileBackgroundLoader :
        CacheBackgroundLoader<ILoadJobProfileContent>
    {
        public JobProfileBackgroundLoader(
            ILogger<JobProfileBackgroundLoader> logger,
            IContentApiConfiguration clientOptions,
            ILoadJobProfileContent cacheLoader,
            IHostedServiceTelemetryWrapper telemetryWrapper)
                : base(logger, clientOptions, cacheLoader, telemetryWrapper)
        {
        }
    }
}
