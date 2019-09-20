using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.WhatItTakesDataModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService
{
    public class WhatItTakesSegmentService : BaseSegmentService<WhatItTakesSegmentModel, WhatItTakesSegmentService>, IWhatItTakesSegmentService
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
