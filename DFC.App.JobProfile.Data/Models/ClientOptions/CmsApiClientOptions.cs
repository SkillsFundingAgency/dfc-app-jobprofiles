namespace DFC.App.JobProfile.Data.Models.ClientOptions
{
    public class CmsApiClientOptions : ClientOptionsModel
    {
        public string SummaryEndpoint { get; set; } = "content/getcontent/api/execute/page";

        public string StaticContentEndpoint { get; set; } = "content/getcontent/api/execute/sharedcontent/";

        public string ContentIds { get; set; }
    }
}
