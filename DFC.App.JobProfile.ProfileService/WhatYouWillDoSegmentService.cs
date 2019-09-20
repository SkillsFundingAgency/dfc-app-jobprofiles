using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments;
using DFC.App.JobProfile.Data.Models.Segments.WhatYouWillDoDataModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService
{
    public class WhatYouWillDoSegmentService : BaseSegmentService<WhatYouWillDoSegmentModel, WhatYouWillDoSegmentService>, IWhatYouWillDoSegmentService
    {
        public WhatYouWillDoSegmentService(
                                            HttpClient httpClient,
                                            ILogger<WhatYouWillDoSegmentService> logger,
                                            WhatYouWillDoSegmentClientOptions whatYouWillDoSegmentClientOptions)
        : base(httpClient, logger, whatYouWillDoSegmentClientOptions)
        {
        }
    }
}
