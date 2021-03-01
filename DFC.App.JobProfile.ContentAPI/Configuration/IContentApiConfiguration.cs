using System;

namespace DFC.App.JobProfile.ContentAPI.Configuration
{
    public interface IContentApiConfiguration
    {
        Uri BaseAddress { get; }

        TimeSpan Timeout { get; }

        string ApiKey { get; }

        string SummaryEndpoint { get; set; }

        string StaticContentEndpoint { get; set; }

        string[] PageStaticContentIDs { get; set; }

        string[] SupportedRelationships { get; set; }
    }
}