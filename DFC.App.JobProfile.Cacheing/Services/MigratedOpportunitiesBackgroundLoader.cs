using DFC.App.JobProfile.ContentAPI.Configuration;
using DFC.Compui.Telemetry.HostedService;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Services
{
    [ExcludeFromCodeCoverage]
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
