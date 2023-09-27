using DFC.App.JobProfile.Data.Contracts;
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

        /// <summary>
        /// Gets or sets the social proof video when one is enabled for the job profile.
        /// </summary>
        /// <value>
        /// A <see cref="SocialProofVideo"/> when present; otherwise, a value of <c>null</c>.
        /// </value>
        public SocialProofVideo SocialProofVideo { get; set; }
    }
}