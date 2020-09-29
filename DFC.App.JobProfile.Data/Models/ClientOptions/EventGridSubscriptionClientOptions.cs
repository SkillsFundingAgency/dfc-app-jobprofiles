using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;

namespace DFC.App.JobProfile.Data.Models.ClientOptions
{
    public class EventGridSubscriptionClientOptions : ClientOptionsModel
    {
        public string Endpoint { get; set; } = "api/execute/";
    }
}
