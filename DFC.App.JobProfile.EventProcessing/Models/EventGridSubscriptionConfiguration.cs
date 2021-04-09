using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.EventProcessing.Models
{
    [ExcludeFromCodeCoverage]
    internal sealed class EventGridSubscriptionConfiguration :
        IEventGridSubscriptionConfiguration
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);

        public string ApiKey { get; set; }

        public string Endpoint { get; set; } = "api/execute/";
    }
}
