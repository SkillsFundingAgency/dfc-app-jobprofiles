using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models.ServiceBusModels
{
    public class JobProfileMessage
    {
        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        [Required]
        public DateTime? LastModified { get; set; }

        public string Title { get; set; }

        public bool IncludeInSitemap { get; set; }

        public string Overview { get; set; }
    }
}
