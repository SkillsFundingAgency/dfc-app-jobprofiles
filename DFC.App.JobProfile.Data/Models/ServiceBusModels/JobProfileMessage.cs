using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models.ServiceBusModels
{
    [ExcludeFromCodeCoverage]
    public class JobProfileMessage
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        [Required]
        public DateTime? LastModified { get; set; }

        public string Title { get; set; }

        public bool IncludeInSitemap { get; set; }

        public string Overview { get; set; }

        /// <summary>
        /// Gets or sets the social proof video when one is enabled for the job profile.
        /// </summary>
        /// <value>
        /// A <see cref="SocialProofVideo"/> when present; otherwise, a value of <c>null</c>.
        /// </value>
        public SocialProofVideo SocialProofVideo { get; set; }
    }
}
