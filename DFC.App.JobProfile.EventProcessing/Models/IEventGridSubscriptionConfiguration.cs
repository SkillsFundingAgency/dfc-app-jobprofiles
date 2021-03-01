using System;

namespace DFC.App.JobProfile.EventProcessing.Models
{
    public interface IEventGridSubscriptionConfiguration
    {
        Uri BaseAddress { get; }

        TimeSpan Timeout { get; }

        string ApiKey { get; }

        string Endpoint { get; }
    }
}