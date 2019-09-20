using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.RelatedCareersDataModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService
{
    public class RelatedCareersSegmentService : BaseSegmentService<RelatedCareersSegmentModel, RelatedCareersSegmentService>, IRelatedCareersSegmentService
    {
        public RelatedCareersSegmentService(
                                                HttpClient httpClient,
                                                ILogger<RelatedCareersSegmentService> logger,
                                                RelatedCareersSegmentClientOptions relatedCareersSegmentClientOptions)
        : base(httpClient, logger, relatedCareersSegmentClientOptions)
        {
        }
    }
}
