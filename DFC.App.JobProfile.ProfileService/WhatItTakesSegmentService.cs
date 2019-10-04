using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileSkillModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService
{
    public class WhatItTakesSegmentService : BaseSegmentService<JobProfileSkillSegmentModel, WhatItTakesSegmentService>, IWhatItTakesSegmentService
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
