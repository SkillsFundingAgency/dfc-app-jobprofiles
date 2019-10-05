using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models.PatchModels
{
    public class JobProfileMetaDataPatchModel
    {
        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public MetaTagsModel MetaTags { get; set; }

        public string BreadcrumbTitle { get; set; }

        public bool IncludeInSitemap { get; set; }

        public string[] AlternativeNames { get; set; }
   }
}
