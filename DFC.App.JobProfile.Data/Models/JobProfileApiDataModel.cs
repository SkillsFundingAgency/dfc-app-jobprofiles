using DFC.Compui.Cosmos.Enums;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileApiDataModel :
        IBaseContentItemModel<JobProfileApiContentItemModel>
    {
        [JsonProperty("id")]
        public Guid ItemId { get; set; } = Guid.Empty;

        [JsonProperty("alias_alias")]
        public string CanonicalName { get; set; } = string.Empty;

        [JsonIgnore]
        public string Pagelocation => $"{TaxonomyTerms.FirstOrDefault() ?? string.Empty}/{CanonicalName}";

        [JsonProperty("taxonomy_terms")]
        public ICollection<string> TaxonomyTerms { get; set; } = new List<string>();

        public Guid Version { get; set; } = Guid.Empty;

        public string BreadcrumbTitle { get; set; } = string.Empty;

        [JsonProperty("sitemap_Exclude")]
        public bool ExcludeFromSitemap { get; set; }

        [JsonIgnore]
        public bool IncludeInSitemap => !ExcludeFromSitemap;

        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Keywords { get; set; } = string.Empty;

        [JsonProperty("_links")]
        public JObject Links { get; set; } = new JObject();

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; } = DateTime.UtcNow;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [JsonProperty("sitemap_Priority")]
        public decimal SiteMapPriority { get; set; }

        [JsonProperty("sitemap_ChangeFrequency")]
        public SiteMapChangeFrequency SiteMapChangeFrequency { get; set; }

        public string RedirectLocations { get; set; } = string.Empty;

        public string SalaryStarter { get; set; } = string.Empty;

        public string SalaryExperienced { get; set; } = string.Empty;

        public string CareerPathAndProgression { get; set; } = string.Empty;

        public decimal MinimumHours { get; set; }

        public string? HtbCareerTips { get; set; }

        public string HtbBodies { get; set; } = string.Empty;

        public Uri JobProfileWebsiteUrl { get; set; } = UriExtra.Empty;

        public string WitDigitalSkillsLevel { get; set; } = string.Empty;

        public string WorkingPattern { get; set; } = string.Empty;

        public string WorkingHoursDetails { get; set; } = string.Empty;

        public string TitleOptions { get; set; } = string.Empty;

        public decimal MaximumHours { get; set; }

        public string WorkingPatternDetails { get; set; } = string.Empty;

        public string HtbFurtherInformation { get; set; } = string.Empty;

        public string FurtherInfo { get; set; } = string.Empty;

        public string RelevantSubjects { get; set; } = string.Empty;

        public JobProfileWhatYoullDoModel? WhatYoullDoSegment { get; set; }

        public JobProfileCareerPathModel? CareerPathSegment { get; set; }

        public JobProfileHowToBecomeModel? HowToBecomeSegment { get; set; }

        public JobProfileWhatItTakesModel? WhatItTakesSegment { get; set; }

        public IList<JobProfileApiContentItemModel> ContentItems { get; set; } = new List<JobProfileApiContentItemModel>();

        public ICollection<Guid> AllContentItemIds { get; set; } = new List<Guid>();

        [JsonIgnore]
        public ContentLinksModel? ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);
            set => PrivateLinksModel = value ?? new ContentLinksModel(Links);
        }

        private ContentLinksModel? PrivateLinksModel { get; set; }

        public List<string> Redirects() =>
            string.IsNullOrEmpty(RedirectLocations) ? new List<string>() : RedirectLocations.Split("\r\n").ToList();
    }
}