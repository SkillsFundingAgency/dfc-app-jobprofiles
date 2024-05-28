using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileModel : BaseJobProfile
    {
        public string BreadcrumbTitle { get; set; }

        [Required]
        public bool IncludeInSitemap { get; set; }

        public IList<string> AlternativeNames { get; set; }

        [Required]
        public MetaTags MetaTags { get; set; }

        public IList<SegmentModel> Segments { get; set; }
    }
}