using System;

namespace DFC.App.JobProfile.Data.HttpClientPolicies
{
    public class SegmentClientOptions
    {
        public Uri BaseAddress { get; set; }

        public string Endpoint { get; set; } = "segment/{0}/contents";

        public string OfflineHtml { get; set; } = "<div><h3>Sorry, there is a problem with the service</h3><p>Try again later.</p></div>";

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);         // default to 30 seconds
    }
}
