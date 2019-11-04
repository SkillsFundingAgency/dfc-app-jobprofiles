using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileMetadata : BaseJobProfile
    {
        public MetaTags MetaTags { get; set; }

        public string BreadcrumbTitle { get; set; }

        public IList<string> AlternativeNames { get; set; }

        public bool IncludeInSitemap { get; set; }
    }
}