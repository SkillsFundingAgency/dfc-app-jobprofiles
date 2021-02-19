﻿using System;

namespace DFC.App.JobProfile.Data.Models.ClientOptions
{
    public class EventGridPublishClientOptions
    {
        public string TopicEndpoint { get; set; }

        public string SubjectPrefix { get; set; }

        public string TopicKey { get; set; }

        public Uri ApiEndpoint { get; set; }
    }
}
