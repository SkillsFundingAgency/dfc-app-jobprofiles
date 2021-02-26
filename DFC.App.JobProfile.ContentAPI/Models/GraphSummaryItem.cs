using Newtonsoft.Json;
using System;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public sealed class GraphSummaryItem :
        IResourceLocatable
    {
        public static readonly Uri Empty = new Uri("about:nothing");

        [JsonProperty("uri")]
        public Uri Uri { get; set; } = Empty;

        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string CanonicalName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.MinValue;

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; } = DateTime.MinValue;
    }
}
