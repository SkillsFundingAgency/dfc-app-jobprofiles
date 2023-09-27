using DFC.App.JobProfile.Data.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data
{
    public class BaseJobProfile
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public long SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the social proof video when one is enabled for the job profile.
        /// </summary>
        /// <value>
        /// A <see cref="SocialProofVideo"/> when present; otherwise, a value of <c>null</c>.
        /// </value>
        public SocialProofVideo SocialProofVideo { get; set; }
    }
}