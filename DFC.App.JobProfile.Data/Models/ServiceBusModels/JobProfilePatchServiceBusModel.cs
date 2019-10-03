﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models.ServiceBusModels
{
    public class JobProfilePatchServiceBusModel
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }
    }
}