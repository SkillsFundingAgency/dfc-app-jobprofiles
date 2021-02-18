using DFC.App.JobProfile.Data.Contracts;
using DFC.Compui.Telemetry.HostedService;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.HostedServices
{
    public class JobProfileBackgroundLoader :
        CacheLoadingBackgroundService<ILoadJobProfileContent>
    {
        public JobProfileBackgroundLoader(
            ILogger<JobProfileBackgroundLoader> logger,
            CmsApiClientOptions clientOptions,
            ILoadJobProfileContent cacheLoader,
            IHostedServiceTelemetryWrapper telemetryWrapper)
                : base(logger, clientOptions, cacheLoader, telemetryWrapper)
        {
        }
    }
}
