using DFC.App.JobProfile.Data.Contracts;
using DFC.Compui.Telemetry.HostedService;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.HostedServices
{
    public class StaticContentBackgroundLoader :
        CacheLoadingBackgroundService<ILoadStaticContent>
    {
        public StaticContentBackgroundLoader(
            ILogger<StaticContentBackgroundLoader> logger,
            CmsApiClientOptions clientOptions,
            ILoadStaticContent cacheLoader,
            IHostedServiceTelemetryWrapper telemetryWrapper)
                : base(logger, clientOptions, cacheLoader, telemetryWrapper)
        {
        }
    }
}
