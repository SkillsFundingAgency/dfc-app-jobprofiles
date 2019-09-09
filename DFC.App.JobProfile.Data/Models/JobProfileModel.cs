using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models.Segments;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileModel : IDataModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public string BreadcrumbTitle { get; set; }

        public bool IncludeInSitemap { get; set; }

        public MetaTagsModel MetaTags { get; set; }

        public SegmentsModel Segments { get; set; }

        public DateTime LastReviewed { get; set; }

        public string[] AlternativeNames { get; set; }
    }
}