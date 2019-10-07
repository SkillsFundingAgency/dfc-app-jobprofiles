using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels
{
    public class RelatedCareerDataModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public string Title { get; set; }

        public string SocLevelTwo { get; set; }
    }
}