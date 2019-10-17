using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models.PatchModels
{
    public class JobProfileMetadata : BaseJobProfile
    {
        [Required]
        public MetaTags MetaTags { get; set; }

        public string BreadcrumbTitle { get; set; }

        public bool IncludeInSitemap { get; set; }

        public IList<string> AlternativeNames { get; set; }
   }
}
