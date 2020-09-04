using DFC.Compui.Cosmos.Enums;
using dfc_content_pkg_netcore.contracts;
using dfc_content_pkg_netcore.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.Data.Models
{
    public class PagesApiDataModel : IApiDataModel, IPagesApiDataModel
    {
        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty("alias_alias")]
        public string? CanonicalName { get; set; }

        [JsonIgnore]
        public string Pagelocation => $"{TaxonomyTerms.FirstOrDefault() ?? string.Empty}/{CanonicalName}";

        [JsonProperty("taxonomy_terms")]
        public List<string> TaxonomyTerms { get; set; } = new List<string>();

        public Guid? Version { get; set; }

        public string? BreadcrumbTitle { get; set; }

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

        public IList<ApiContentItemModel> ContentItems { get; set; } = new List<ApiContentItemModel>();

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

        public string FurtherInfo { get; set; }

        public string RelevantSubjects { get; set; }
    }
}
