using DFC.App.Services.Common.Registration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.EventProcessing.Models
{
    [ExcludeFromCodeCoverage]
    internal sealed class EventGridPublicationConfiguration :
        IEventGridPublicationConfiguration,
        IRequireConfigurationRegistration
    {
        public string TopicEndpoint { get; set; }

        public string SubjectPrefix { get; set; }

        public string TopicKey { get; set; }

        public Uri ApiEndpoint { get; set; }
    }
}
