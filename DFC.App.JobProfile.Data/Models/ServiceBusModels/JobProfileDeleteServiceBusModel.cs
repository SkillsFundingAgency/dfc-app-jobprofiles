using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models.ServiceBusModels
{
    public class JobProfileDeleteServiceBusModel
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public DateTime LastReviewed { get; set; }
    }
}
