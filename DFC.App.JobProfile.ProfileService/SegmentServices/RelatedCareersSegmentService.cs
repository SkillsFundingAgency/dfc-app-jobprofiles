using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService.SegmentServices
{
    public class RelatedCareersSegmentService : BaseSegmentService<RelatedCareersSegmentDataModel, RelatedCareersSegmentService>, IRelatedCareersSegmentService
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
