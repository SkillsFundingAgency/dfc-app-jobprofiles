using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService
{
    public class CurrentOpportunitiesSegmentService : BaseSegmentService<CurrentOpportunitiesSegmentModel, CurrentOpportunitiesSegmentService>, ICurrentOpportunitiesSegmentService
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
