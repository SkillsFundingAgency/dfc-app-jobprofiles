﻿using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileTasksModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService.SegmentServices
{
    public class WhatYouWillDoSegmentService : BaseSegmentService<JobProfileTasksSegmentDataModel, WhatYouWillDoSegmentService>, IWhatYouWillDoSegmentService
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