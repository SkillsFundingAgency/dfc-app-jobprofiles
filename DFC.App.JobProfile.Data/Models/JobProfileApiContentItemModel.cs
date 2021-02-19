using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileApiContentItemModel :
        IBranchContentItemModel<JobProfileApiContentItemModel>
    {
        [JsonProperty("uri")]
        public Uri Uri { get; set; } = UriExtra.Empty;

        public string Description { get; set; } = string.Empty;

        public string FurtherInfo { get; set; } = string.Empty;

        public string RelevantSubjects { get; set; } = string.Empty;

        [JsonProperty("id")]
        public Guid ItemId { get; set; } = Guid.Empty;

        [JsonProperty("skos__prefLabel")]
        public string Content { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; } = DateTime.UtcNow;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string Title { get; set; } = string.Empty;

        [JsonProperty("contentType")]
        public string ContentType { get; set; } = string.Empty;

        [JsonProperty("_links")]
        public JObject Links { get; set; } = new JObject();

        public ICollection<JobProfileApiContentItemModel> ContentItems { get; set; } = new List<JobProfileApiContentItemModel>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BreadcrumbLinkSegment { get; set; } = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BreadcrumbText { get; set; } = string.Empty;

        public DateTime LastCached { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ContentLinksModel ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value ??= new ContentLinksModel(Links);
        }

        [JsonIgnore]
        private ContentLinksModel PrivateLinksModel { get; set; } = new ContentLinksModel(null);

        public bool IsFaultedState() =>
            Uri == UriExtra.Empty
                || ItemId == Guid.Empty
                || Title == string.Empty
                || Content == string.Empty;
    }
}
