using Newtonsoft.Json;
using System;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    internal sealed class GraphItem :
        IGraphItem
    {
        [JsonIgnore]
        public Uri Uri { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        public int Ordinal { get; set; }
    }
}
