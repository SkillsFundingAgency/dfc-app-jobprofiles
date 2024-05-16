using DFC.App.JobProfile.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileModel : BaseJobProfile
    {

        [Required]
        public string SocLevelTwo { get; set; }

        [Required]
        public DateTime? LastReviewed { get; set; }

        public string BreadcrumbTitle { get; set; }

        [Required]
        public bool IncludeInSitemap { get; set; }

        public IList<string> AlternativeNames { get; set; }

        [Required]
        public MetaTags MetaTags { get; set; }

        public IList<SegmentModel> Segments { get; set; }
    }
}