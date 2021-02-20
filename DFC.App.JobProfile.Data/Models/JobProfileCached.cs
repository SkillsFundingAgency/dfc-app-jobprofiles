using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Cosmos.Models;
using DFC.Compui.Telemetry.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileCached :
        RequestTrace,
        IJobProfileCached
    {
        [Display(Name = "Breadcrumb Title")]
        public string BreadcrumbTitle { get; set; } = string.Empty;

        [JsonProperty("id")]
        public Guid Id { get; set; }

        public Guid Version { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [Display(Name = "SiteMap Priority")]
        public double SiteMapPriority { get; set; }

        [Required]
        [Display(Name = "Last Reviewed")]
        public DateTime LastReviewed { get; set; }

        public MetaTagsModel MetaTags { get; set; } = new MetaTagsModel();

        [JsonProperty("sitemap_Exclude")]
        public bool ExcludeFromSitemap { get; set; }

        [JsonIgnore]
        public bool IncludeInSitemap => !ExcludeFromSitemap;

        //[JsonProperty("skos__prefLabel")]
        public string CanonicalName { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public bool IsDefaultForPageLocation { get; set; }

        public string PageLocation { get; set; }

        public string WitDigitalSkillsLevel { get; set; } = string.Empty;

        public string TitleOptions { get; set; } = string.Empty;

        public string HtbFurtherInformation { get; set; } = string.Empty;

        public JobProfileCachedOverview OverviewSegment { get; set; }

        public JobProfileCachedWhatYoullDo WhatYoullDoSegment { get; set; }

        public JobProfileCachedCareerPath CareerPathSegment { get; set; }

        public JobProfileCachedHowToBecome HowToBecomeSegment { get; set; }

        public JobProfileCachedWhatItTakes WhatItTakesSegment { get; set; }

        [JsonIgnore]
        public ICollection<Guid> AllContentItemIds { get; set; } = new List<Guid>();

        public IList<string> RedirectLocations { get; set; }

        public string Etag { get; set; }

        public string PartitionKey { get; set; }

        Guid? IContentPageModel.Version { get => Version; set => Version = value.Value; }
    }
}
