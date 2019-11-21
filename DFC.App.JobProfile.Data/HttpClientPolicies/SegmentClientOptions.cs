using System;

namespace DFC.App.JobProfile.Data.HttpClientPolicies
{
    public class SegmentClientOptions
    {
        public Uri BaseAddress { get; set; }

        public string Endpoint { get; set; } = "segment/{0}/contents";

        public string OfflineHtml { get; set; }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);         // default to 10 seconds
    }
}
