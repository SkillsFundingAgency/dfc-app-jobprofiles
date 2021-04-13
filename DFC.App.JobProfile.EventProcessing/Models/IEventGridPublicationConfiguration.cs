using System;

namespace DFC.App.JobProfile.EventProcessing.Models
{
    internal interface IEventGridPublicationConfiguration
    {
        string TopicEndpoint { get; set; }

        string SubjectPrefix { get; set; }

        string TopicKey { get; set; }

        Uri ApiEndpoint { get; set; }
    }
}