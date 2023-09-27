using DFC.App.JobProfile.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.ViewModels
{
    public class DocumentViewModel
    {
        public HeadViewModel Head { get; set; }

        public BreadcrumbViewModel Breadcrumb { get; set; }

        public BodyViewModel Body { get; set; }

        [Display(Name = "Document Id")]
        public Guid? DocumentId { get; set; }

        [Display(Name = "Canonical Name")]
        public string CanonicalName { get; set; }

        [Display(Name = "Breadcrumb Title")]
        public string BreadcrumbTitle { get; set; }

        [Display(Name = "Include In SiteMap")]
        public bool IncludeInSitemap { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime LastReviewed { get; set; }

        public long SequenceNumber { get; set; }

        [Display(Name = "Alternative Names")]
        public string[] AlternativeNames { get; set; }

        /// <summary>
        /// Gets or sets the social proof video when one is enabled for the job profile.
        /// </summary>
        /// <value>
        /// A <see cref="SocialProofVideo"/> when present; otherwise, a value of <c>null</c>.
        /// </value>
        public SocialProofVideo SocialProofVideo { get; set; }
    }
}