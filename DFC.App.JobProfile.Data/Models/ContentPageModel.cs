using DFC.Compui.Cosmos.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DFC.App.JobProfile.Data.Models
{
    public class ContentPageModel : Compui.Cosmos.Models.ContentPageModel
    {
        [Required]
        [JsonProperty(Order = -10)]
        public override string? PartitionKey => PageLocation;

        public override string? PageLocation { get; set; } = "/missing-location";

        public new string? Content { get; set; }

        [Display(Name = "Breadcrumb Title")]
        public new string? BreadcrumbTitle { get; set; }

        [JsonProperty(Order = -10)]
        public new Guid? Version { get; set; }

        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty("alias_alias")]
        public string? CanonicalName { get; set; }

        [JsonProperty("taxonomy_terms")]
        public List<string> TaxonomyTerms { get; set; } = new List<string>();

        [JsonProperty("sitemap_Exclude")]
        public bool ExcludeFromSitemap { get; set; }

        [JsonIgnore]
        public bool IncludeInSitemap => !ExcludeFromSitemap;

        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Keywords { get; set; }

        [JsonProperty("_links")]
        public JObject? Links { get; set; }

        [JsonIgnore]
        public ContentLinksModel? ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value;
        }


        public IList<PagesApiContentItemModel> ContentItems { get; set; } = new List<PagesApiContentItemModel>();

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty("sitemap_Priority")]
        public decimal SiteMapPriority { get; set; }

        [JsonProperty("sitemap_ChangeFrequency")]
        public SiteMapChangeFrequency SiteMapChangeFrequency { get; set; }

        public string RedirectLocations { get; set; } = string.Empty;

        public List<string> Redirects()
        {
            return string.IsNullOrEmpty(RedirectLocations) ? new List<string>() : RedirectLocations.Split("\r\n").ToList();
        }


        //////////////////////
        public string SalaryStarter { get; set; }

        public string SalaryExperienced { get; set; }

        public decimal MinimumHours { get; set; }

        public string? HtbCareerTips { get; set; }

        public string HtbBodies { get; set; }

        public Uri JobProfileWebsiteUrl { get; set; }

        public string WitDigitalSkillsLevel { get; set; }

        public string WorkingPattern { get; set; }

        ////public DateTime ModifiedDate { get; set; }

        public string WorkingHoursDetails { get; set; }

        public string TitleOptions { get; set; }

        public decimal MaximumHours { get; set; }

        public string WorkingPatternDetails { get; set; }

        public string CareerPathAndProgression { get; set; }

        public string HtbFurtherInformation { get; set; }

        private ContentLinksModel? PrivateLinksModel { get; set; }
    }
}
