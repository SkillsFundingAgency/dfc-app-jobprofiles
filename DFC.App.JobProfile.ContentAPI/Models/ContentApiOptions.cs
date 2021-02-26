using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public sealed class ContentApiOptions :
        ClientOptionsModel
    {
        public string SummaryEndpoint { get; set; } = "/page";

        public string StaticContentEndpoint { get; set; } = "/sharedcontent/";

        public string[] PageStaticContentIDs { get; set; }

        public string[] SupportedRelationships { get; set; }
    }
}
