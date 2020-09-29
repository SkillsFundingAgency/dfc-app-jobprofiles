using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileApiContentItemModel : IBaseContentItemModel<JobProfileApiContentItemModel>
    {

        [JsonProperty("uri")]
        public Uri Url { get; set; }

        public string Description { get; set; }

        public string FurtherInfo { get; set; }

        public string RelevantSubjects { get; set; }

        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string? Content { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Title { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("_links")]
        public JObject? Links { get; set; }

        public IList<JobProfileApiContentItemModel> ContentItems { get; set; } = new List<JobProfileApiContentItemModel>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? BreadcrumbLinkSegment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? BreadcrumbText { get; set; }

        public DateTime LastCached { get; set; } = DateTime.UtcNow;


        [JsonIgnore]
        public ContentLinksModel? ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value;
        }

        [JsonIgnore]
        private ContentLinksModel PrivateLinksModel { get; set; }
    }
}
