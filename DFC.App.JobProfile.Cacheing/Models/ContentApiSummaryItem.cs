using DFC.App.JobProfile.ContentAPI.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    internal sealed class ContentApiSummaryItem :
        IGraphSummaryItem
    {
        [JsonProperty("uri")]
        public Uri Uri { get; set; } = UriExtra.Empty;

        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string CanonicalName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.MinValue;

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime ModifiedDateTime { get; set; } = DateTime.MinValue;
    }
}
