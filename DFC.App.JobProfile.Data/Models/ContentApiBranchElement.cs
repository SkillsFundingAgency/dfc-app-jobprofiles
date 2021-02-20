using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class ContentApiBranchElement :
        IBranchContentItems<ContentApiBranchElement>,
        IContentApiBranchElement
    {
        [JsonProperty("uri")]
        public Uri Uri { get; set; } = UriExtra.Empty;

        public string Description { get; set; } = string.Empty;

        public string FurtherInfo { get; set; } = string.Empty;

        public string RelevantSubjects { get; set; } = string.Empty;

        [JsonProperty("id")]
        public Guid ItemID { get; set; } = Guid.Empty;

        [JsonProperty("skos__prefLabel")]
        public string Content { get; set; } = string.Empty;

        [JsonProperty("ModifiedDate")]
        public DateTime Published { get; set; } = DateTime.UtcNow;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [JsonProperty("contentType")]
        public string ContentType { get; set; } = string.Empty;

        [JsonProperty("_links")]
        public JObject Links { get; set; } = new JObject();

        public ICollection<ContentApiBranchElement> ContentItems { get; set; } = new List<ContentApiBranchElement>();

        IEnumerable<IContentApiBranchElement> IContentApiBranchElement.ContentItems => ContentItems;

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
        public string CanonicalName { get; set; }

        [JsonIgnore]
        public string PageLocation { get; set; }

        [JsonIgnore]
        public IList<string> RedirectLocations { get; set; }

        [JsonIgnore]
        public Guid? Version { get; set; }

        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string Etag { get; set; }

        [JsonIgnore]
        public string PartitionKey { get; set; }

        [JsonIgnore]
        public string TraceId => throw new NotImplementedException();

        [JsonIgnore]
        public string ParentId => throw new NotImplementedException();

        [JsonIgnore]
        private ContentLinksModel PrivateLinksModel { get; set; } = new ContentLinksModel(null);

        public void AddParentId(string parentId)
        {
            throw new NotImplementedException();
        }

        public void AddTraceId(string traceId)
        {
            throw new NotImplementedException();
        }

        public bool IsFaultedState() =>
            Uri == UriExtra.Empty
                || ItemID == Guid.Empty
                || CanonicalName == string.Empty
                || Content == string.Empty;
    }
}
