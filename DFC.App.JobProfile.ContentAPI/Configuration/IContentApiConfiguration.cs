using System;

namespace DFC.App.JobProfile.ContentAPI.Configuration
{
    public interface IContentApiConfiguration
    {
        Uri BaseAddress { get; }

        TimeSpan Timeout { get; }

        string ApiKey { get; }

        string SummaryEndpoint { get; }

        string StaticContentEndpoint { get; }

        string[] PageStaticContentIDs { get; }

        string[] RelationshipStubs { get; }

        string[] SupportedRelationships { get; }
    }
}