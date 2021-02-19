using DFC.Compui.Cosmos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileContentPageModel : ContentPageModel
    {
        [Required]
        [JsonProperty(Order = -30)]
        public override string PartitionKey => Title!;

        //public new string? Content { get; set; }

        [Display(Name = "Breadcrumb Title")]
        public string BreadcrumbTitle { get; set; } = string.Empty;

        //[JsonProperty(Order = -10)]
        //public new Guid? Version { get; set; }

        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty("sitemap_Exclude")]
        public bool ExcludeFromSitemap { get; set; }

        [JsonIgnore]
        public new bool IncludeInSitemap => !ExcludeFromSitemap;

        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Keywords { get; set; }

        //[JsonProperty("_links")]
        //[JsonIgnore]
        //public JObject? Links { get; set; }

        //[JsonIgnore]
        //public ContentLinksModel? ContentLinks
        //{
        //    get => PrivateLinksModel ??= new ContentLinksModel(Links);

        //    set => PrivateLinksModel = value;
        //}

        //public List<JobProfileApiContentItemModel> ContentItems { get; set; } = new List<JobProfileApiContentItemModel>();

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        //public DateTime? CreatedDate { get; set; }

        //public string RedirectLocations { get; set; } = string.Empty;

        //public List<string> Redirects()
        //{
        //    return string.IsNullOrEmpty(RedirectLocations) ? new List<string>() : RedirectLocations.Split("\r\n").ToList();
        //}

        public Uri JobProfileWebsiteUrl { get; set; }

        public override string? PageLocation { get; set; }

        public string WitDigitalSkillsLevel { get; set; } = string.Empty;

        public string TitleOptions { get; set; } = string.Empty;

        public string HtbFurtherInformation { get; set; } = string.Empty;

        public JobProfileOverviewModel? OverviewSegment { get; set; }

        public JobProfileWhatYoullDoModel? WhatYoullDoSegment { get; set; }

        public JobProfileCareerPathModel? CareerPathSegment { get; set; }

        public JobProfileHowToBecomeModel? HowToBecomeSegment { get; set; }

        public JobProfileWhatItTakesModel? WhatItTakesSegment { get; set; }

        //[JsonIgnore]
        //private ContentLinksModel? PrivateLinksModel { get; set; }

        //[JsonIgnore]
        //public List<Guid> AllContentItemIds => ContentItems.Flatten(s => s.ContentItems).Where(w => w.ItemId != null).Select(s => s.ItemId!.Value).ToList();

        [JsonIgnore]
        public ICollection<Guid> AllContentItemIds { get; set; } = new List<Guid>();
    }
}
