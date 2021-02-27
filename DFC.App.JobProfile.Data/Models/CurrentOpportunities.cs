using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Telemetry.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public sealed class CurrentOpportunities :
        RequestTrace,
        IContentPageModel
    {
        public string Title { get; set; }

        public IReadOnlyCollection<OpportunityApprenticeship> Apprenticeships { get; set; }

        public IReadOnlyCollection<OpportunityCourse> Courses { get; set; }

        #region inherited from request trace and content page model

        public string CanonicalName { get; set; }

        public string PageLocation { get; set; }

        public IList<string> RedirectLocations { get; set; }

        public Guid? Version { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        public string Etag { get; set; }

        public string PartitionKey { get; set; }

        #endregion inherited from request trace and content page model
    }
}
