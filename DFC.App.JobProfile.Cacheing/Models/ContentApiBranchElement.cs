using DFC.App.JobProfile.ContentAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    // i've left json property mappings in to re-enforce
    // where the data values are coming from
    public class ContentApiBranchElement :
        IBranchContentItem<ContentApiBranchElement>,
        IContentApiBranchElement
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        // auto-mapped from graph item
        public Uri Uri { get; set; } = UriExtra.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [JsonProperty("ModifiedDate")]
        public DateTime Published { get; set; } = DateTime.UtcNow;

        [JsonProperty("_links")]
        public JObject Curies { get; set; }

        public ICollection<ContentApiBranchElement> ContentItems { get; set; } = new List<ContentApiBranchElement>();

        // auto-mapped from graph item
        public string Title { get; set; }

        // auto-mapped from graph item
        public string ContentType { get; set; }

        // auto-mapped from graph item
        public int Ordinal { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Text")]
        public string Text { get; set; }

        [JsonProperty("Link_text")]
        public string LinkText { get; set; }

        [JsonProperty("Link_url")]
        public string Link { get; set; }

        [JsonProperty("FurtherInfo")]
        public string FurtherInformation { get; set; }

        [JsonProperty("RelevantSubjects")]
        public string RelevantSubjects { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Reference")]
        public string Reference { get; set; }

        [JsonProperty("Duration")]
        public int Duration { get; set; }

        [JsonProperty("LARSCode")]
        public int LARSCode { get; set; }

        [JsonProperty("MaximumFunding")]
        public int MaximumFunding { get; set; }

        public bool IsFaultedState() =>
            Uri == UriExtra.Empty
            || Id == Guid.Empty;
    }
}
