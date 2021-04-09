using DFC.App.JobProfile.ContentAPI.Configuration;
using DFC.Compui.Telemetry.HostedService;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Services
{
    [ExcludeFromCodeCoverage]
    internal sealed class StaticContentBackgroundLoader :
        CacheBackgroundLoader<ILoadStaticContent>
    {
        public StaticContentBackgroundLoader(
            ILogger<StaticContentBackgroundLoader> logger,
            IContentApiConfiguration clientOptions,
            ILoadStaticContent cacheLoader,
            IHostedServiceTelemetryWrapper telemetryWrapper)
                : base(logger, clientOptions, cacheLoader, telemetryWrapper)
        {
        }
    }
}
