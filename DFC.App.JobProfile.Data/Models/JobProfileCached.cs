using DFC.Compui.Telemetry.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileCached :
        RequestTrace,
        IJobProfileCached
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        public Guid? Version { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        public DateTime Published { get; set; }

        public string CanonicalName { get; set; }

        public string PageLocation { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        public string WitDigitalSkillsLevel { get; set; }

        public string TitleOptions { get; set; }

        public JobProfileCachedOverview OverviewSegment { get; set; }

        public JobProfileCachedWhatYoullDo WhatYoullDoSegment { get; set; }

        public JobProfileCachedCareerPath CareerPathSegment { get; set; }

        public JobProfileCachedHowToBecome HowToBecomeSegment { get; set; }

        public JobProfileCachedWhatItTakes WhatItTakesSegment { get; set; }

        #region required by site map controller

        [JsonIgnore]
        public bool IncludeInSitemap => true;

        #endregion required by site map controller

        #region required by request trace

        public IList<string> RedirectLocations { get; set; }

        public string Etag { get; set; }

        public string PartitionKey { get; set; }

        #endregion required by request trace
    }
}
