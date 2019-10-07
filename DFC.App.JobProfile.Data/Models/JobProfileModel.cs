using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models.Segments;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileModel : IDataModel
    {
        [Required]
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        public string PartitionKey => SocLevelTwo;

        [Required]
        public DateTime? LastReviewed { get; set; }

        public string BreadcrumbTitle { get; set; }

        public bool IncludeInSitemap { get; set; }

        public string[] AlternativeNames { get; set; }

        public MetaTagsModel MetaTags { get; set; }

        public SegmentsMarkupModel Markup { get; set; }

        public SegmentsDataModel Data { get; set; }
    }
}