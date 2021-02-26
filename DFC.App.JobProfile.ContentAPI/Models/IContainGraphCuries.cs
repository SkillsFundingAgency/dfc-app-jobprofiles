using Newtonsoft.Json.Linq;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public interface IContainGraphCuries
    {
        JObject Curies { get; }
    }
}
