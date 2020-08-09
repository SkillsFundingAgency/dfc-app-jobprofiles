using DFC.App.JobProfile.Data.Contracts;
using Newtonsoft.Json;
using System;

namespace DFC.App.JobProfile.Data.Models
{
    public class PagesSummaryItemModel : IApiDataModel
    {
        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string? Title { get; set; }

        public string? CanonicalName
        {
            get => Title;

            set => Title = value;
        }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }
    }
}
