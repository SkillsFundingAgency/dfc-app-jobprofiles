using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.HowToBecomeModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService.SegmentServices
{
    public class HowToBecomeSegmentService : BaseSegmentService<HowToBecomeSegmentDataModel, HowToBecomeSegmentService>, IHowToBecomeSegmentService
    {
        public HowToBecomeSegmentService(
                                            HttpClient httpClient,
                                            ILogger<HowToBecomeSegmentService> logger,
                                            HowToBecomeSegmentClientOptions howToBecomeSegmentClientOptions)
        : base(httpClient, logger, howToBecomeSegmentClientOptions)
        {
        }
    }
}
