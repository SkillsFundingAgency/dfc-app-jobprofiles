using DFC.App.JobProfile.ContentAPI.Models;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Telemetry.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class StaticItemCached :
        RequestTrace,
        IContentPageModel,
        IResourceLocatable
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        public Guid? Version { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        public DateTime Published { get; set; }

        public string CanonicalName { get; set; }

        public string PageLocation { get; set; } = "/shared-content";

        public string Title { get; set; }

        public string Content { get; set; }

        #region required by request trace

        public IList<string> RedirectLocations { get; set; }

        public string Etag { get; set; }

        public string PartitionKey { get; set; }

        #endregion required by request trace
    }
}
