using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class RefreshJobProfileSegmentModel
    {
        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public string Segment { get; set; }
    }
}
