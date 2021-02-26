using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;

namespace DFC.App.JobProfile.EventProcessing.Models
{
    public class EventGridSubscriptionClientOptions :
        ClientOptionsModel
    {
        public string Endpoint { get; set; } = "api/execute/";
    }
}
