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
    }
}