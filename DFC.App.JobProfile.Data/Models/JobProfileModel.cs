using DFC.App.JobProfile.Data.Contracts;
using dfc_content_pkg_netcore.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileModel : BaseJobProfile, IDataModel
    {
        [Required]
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId
        {
            get => JobProfileId;
            set => JobProfileId = value;
        }

        [Required]
        public string SocLevelTwo { get; set; }

        public string PartitionKey => SocLevelTwo.ToString();

        [Required]
        public DateTime? LastReviewed { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; set; }

        public string BreadcrumbTitle { get; set; }

        [Required]
        public bool IncludeInSitemap { get; set; }

        public IList<string> AlternativeNames { get; set; }

        [Required]
        public MetaTags MetaTags { get; set; }

        public IList<SegmentModel> Segments { get; set; }

        public IList<ApiContentItemModel> ContentItems { get; set; }

        public string JobProfileWebsiteUrl { get; set; }

        public string PageLocation { get; set; }

        public string skos__prefLabel { get; set; }

        public string Description { get; set; }

        public DateTime ModifiedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? SalaryStarter { get; set; }

        public string? SalaryExperienced { get; set; }

        public int? MinimumHours { get; set; }

        public string HtbCareerTips { get; set; }

        public string HtbBodies { get; set; }

        public string WitDigitalSkillsLevel { get; set; }

        public string WorkingPattern { get; set; }

        public string WorkingHoursDetails { get; set; }

        public string TitleOptions { get; set; }

        public int? MaximumHours { get; set; }

        public string WorkingPatternDetails { get; set; }

        public string CareerPathAndProgression { get; set; }

        public string HtbFurtherInformation { get; set; }

        public List<StaticContentItemModel> SharedContent { get; set; }
    }
}