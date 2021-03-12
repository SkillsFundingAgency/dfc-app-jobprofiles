using DFC.App.JobProfile.ContentAPI.Configuration;
using DFC.Compui.Telemetry.HostedService;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.Cacheing.Services
{
    internal sealed class MigratedOpportunitiesBackgroundLoader :
        CacheBackgroundLoader<IMigrateCurrentOpportunities>
    {
        public MigratedOpportunitiesBackgroundLoader(
            ILogger<MigratedOpportunitiesBackgroundLoader> logger,
            IContentApiConfiguration clientOptions,
            IMigrateCurrentOpportunities cacheLoader,
            IHostedServiceTelemetryWrapper telemetryWrapper)
                : base(logger, clientOptions, cacheLoader, telemetryWrapper)
        {
        }
    }
}
