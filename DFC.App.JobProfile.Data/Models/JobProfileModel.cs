using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models.Segments;
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

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; set; }

        [Required]
        public int SocLevelTwo { get; set; }

        public string PartitionKey => SocLevelTwo.ToString();

        public DateTime? LastReviewed { get; set; }


        public string BreadcrumbTitle { get; set; }

        public bool IncludeInSitemap { get; set; }

        public IList<string> AlternativeNames { get; set; }

        public MetaTags MetaTags { get; set; }

        //public SegmentsMarkupModel Markup { get; set; }

        //public SegmentsDataModel Data { get; set; }

        public IList<SegmentModel> Segments { get; set; }
    }
}