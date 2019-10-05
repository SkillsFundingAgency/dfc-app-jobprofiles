using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileSkillModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService.SegmentServices
{
    public class WhatItTakesSegmentService : BaseSegmentService<JobProfileSkillSegmentDataModel, WhatItTakesSegmentService>, IWhatItTakesSegmentService
    {
        public WhatItTakesSegmentService(
                                            HttpClient httpClient,
                                            ILogger<WhatItTakesSegmentService> logger,
                                            WhatItTakesSegmentClientOptions whatItTakesSegmentClientOptions)
        : base(httpClient, logger, whatItTakesSegmentClientOptions)
        {
        }
    }
}
