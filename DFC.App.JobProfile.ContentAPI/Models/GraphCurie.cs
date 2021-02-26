using Newtonsoft.Json;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    internal sealed class GraphCurie
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
