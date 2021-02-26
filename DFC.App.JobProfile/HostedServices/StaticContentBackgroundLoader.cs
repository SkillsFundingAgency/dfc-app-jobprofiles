using DFC.App.JobProfile.Cacheing;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.Compui.Telemetry.HostedService;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.HostedServices
{
    public class StaticContentBackgroundLoader :
        CacheBackgroundLoader<ILoadStaticContent>
    {
        public StaticContentBackgroundLoader(
            ILogger<StaticContentBackgroundLoader> logger,
            ContentApiOptions clientOptions,
            ILoadStaticContent cacheLoader,
            IHostedServiceTelemetryWrapper telemetryWrapper)
                : base(logger, clientOptions, cacheLoader, telemetryWrapper)
        {
        }
    }
}
