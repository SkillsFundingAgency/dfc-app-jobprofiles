using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models.ServiceBusModels
{
    public class RefreshJobProfileSegmentServiceBusModel
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public string Segment { get; set; }
    }
}
