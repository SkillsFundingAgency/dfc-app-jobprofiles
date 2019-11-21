using System;

namespace DFC.App.JobProfile.MessageFunctionApp.HttpClientPolicies
{
    public class JobProfileClientOptions
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);         // default to 10 seconds
    }
}
