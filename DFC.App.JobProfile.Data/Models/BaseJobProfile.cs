using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class BaseJobProfile
    {
        [Required]
        public string CanonicalName { get; set; }

        /// <summary>
        /// Gets or sets the social proof video when one is enabled for the job profile.
        /// </summary>
        /// <value>
        /// A <see cref="SocialProofVideo"/> when present; otherwise, a value of <c>null</c>.
        /// </value>
        public SocialProofVideo Video { get; set; }
    }
}