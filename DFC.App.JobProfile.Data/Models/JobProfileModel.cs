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

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public int PartitionKey => Created.Second;

        public DateTime Updated { get; set; }

        public MetaTagsModel MetaTags { get; set; }

        public string[] AlternativeNames { get; set; }

        public SegmentsMarkupModel Markup { get; set; }

        public SegmentsDataModel Data { get; set; }
    }
}