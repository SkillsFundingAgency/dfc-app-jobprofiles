using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Telemetry.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class SegmentCurrentOpportunity :
        RequestTrace,
        IContentPageModel
    {
        [ExcludeFromCodeCoverage]
        public SegmentData Data { get; set; }

        #region inherited from request trace and content page model

        [ExcludeFromCodeCoverage]
        public string CanonicalName { get; set; }

        [ExcludeFromCodeCoverage]
        public string PageLocation { get; set; }

        [ExcludeFromCodeCoverage]
        public IList<string> RedirectLocations { get; set; }

        [ExcludeFromCodeCoverage]
        public Guid? Version { get; set; }

        [ExcludeFromCodeCoverage]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [ExcludeFromCodeCoverage]
        public string Etag { get; set; }

        [ExcludeFromCodeCoverage]
        public string PartitionKey { get; set; }

        #endregion inherited from request trace and content page model
    }
}
