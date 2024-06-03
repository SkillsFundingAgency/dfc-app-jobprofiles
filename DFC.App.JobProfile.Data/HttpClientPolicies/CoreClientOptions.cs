using System;

namespace DFC.App.JobProfile.Data.HttpClientPolicies
{
    public class CoreClientOptions
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 30);         // default to 30 seconds
    }
}
