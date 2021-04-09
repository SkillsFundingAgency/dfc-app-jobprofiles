using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    [ExcludeFromCodeCoverage]
    internal sealed class GraphCurie
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
