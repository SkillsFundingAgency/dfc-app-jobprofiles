using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.CurrentOpportunitiesModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService.SegmentServices
{
    public class CurrentOpportunitiesSegmentService : BaseSegmentService<CurrentOpportunitiesSegmentDataModel, CurrentOpportunitiesSegmentService>, ICurrentOpportunitiesSegmentService
    {
        public CurrentOpportunitiesSegmentService(
                                                    HttpClient httpClient,
                                                    ILogger<CurrentOpportunitiesSegmentService> logger,
                                                    CurrentOpportunitiesSegmentClientOptions currentOpportunitiesSegmentClientOptions)
        : base(httpClient, logger, currentOpportunitiesSegmentClientOptions)
        {
        }
    }
}
