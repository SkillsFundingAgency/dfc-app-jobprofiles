using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.HowToBecomeModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService
{
    public class HowToBecomeSegmentService : BaseSegmentService<HowToBecomeSegmentModel, HowToBecomeSegmentService>, IHowToBecomeSegmentService
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
