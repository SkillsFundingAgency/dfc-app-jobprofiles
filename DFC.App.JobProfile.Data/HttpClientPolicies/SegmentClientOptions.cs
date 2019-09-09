using System;

namespace DFC.App.JobProfile.Data.HttpClientPolicies
{
    public class SegmentClientOptions
    {
        public Uri BaseAddress { get; set; }

        public string Endpoint { get; set; }

        public string OfflineHtml { get; set; }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 30);         // default to 30 seconds
    }
}
