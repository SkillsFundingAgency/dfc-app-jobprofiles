using DFC.App.JobProfile.ContentAPI.Models;
using Newtonsoft.Json;
using System;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ContentApiStaticElement :
        IResourceLocatable
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string Title { get; set; }

        public string Content { get; set; }

        [JsonProperty("ModifiedDate")]
        public DateTime Published { get; set; }
    }
}
