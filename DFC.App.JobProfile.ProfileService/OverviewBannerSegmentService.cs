using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.OverviewBannerModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService
{
    public class OverviewBannerSegmentService : BaseSegmentService<OverviewBannerSegmentModel, OverviewBannerSegmentService>, IOverviewBannerSegmentService
    {
        public OverviewBannerSegmentService(
                                                HttpClient httpClient,
                                                ILogger<OverviewBannerSegmentService> logger,
                                                OverviewBannerSegmentClientOptions overviewBannerSegmentClientOptions)
        : base(httpClient, logger, overviewBannerSegmentClientOptions)
        {
        }
    }
}
