using DFC.App.JobProfile.Cacheing;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.Compui.Telemetry.HostedService;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.HostedServices
{
    public class JobProfileBackgroundLoader :
        CacheBackgroundLoader<ILoadJobProfileContent>
    {
        public JobProfileBackgroundLoader(
            ILogger<JobProfileBackgroundLoader> logger,
            ContentApiOptions clientOptions,
            ILoadJobProfileContent cacheLoader,
            IHostedServiceTelemetryWrapper telemetryWrapper)
                : base(logger, clientOptions, cacheLoader, telemetryWrapper)
        {
        }
    }
}
