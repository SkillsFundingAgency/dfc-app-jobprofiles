using DFC.App.JobProfile.Data.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class ApiContentItemModel : IApiDataModel
    {
        public Uri? Url { get; set; }

        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        public string Description { get; set; }

        public string FurtherInfo { get; set; }

        public string RelevantSubjects { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string? Content { get; set; }

        public string? ContentType { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty("_links")]
        public JObject? Links { get; set; }

        [JsonIgnore]
        public ContentLinksModel? ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value;
        }

        public IList<ApiContentItemModel> ContentItems { get; set; } = new List<ApiContentItemModel>();

        [JsonIgnore]
        private ContentLinksModel PrivateLinksModel { get; set; }
    }
}
