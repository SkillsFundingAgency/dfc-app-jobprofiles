using DFC.App.Services.Common.Registration;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ContentAPI.Configuration
{
    [ExcludeFromCodeCoverage]
    internal sealed class ContentApiConfiguration :
        ClientOptionsModel,
        IContentApiConfiguration,
        IRequireConfigurationRegistration
    {
        public string SummaryEndpoint { get; set; } = "/page";

        public string StaticContentEndpoint { get; set; } = "/sharedcontent/";

        public string[] PageStaticContentIDs { get; set; }

        public string[] RelationshipStubs { get; set; }

        public string[] SupportedRelationships { get; set; }
    }
}
