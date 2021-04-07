using Newtonsoft.Json.Linq;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public interface IContainGraphLink
    {
        JObject ContentLinks { get; }
    }
}
