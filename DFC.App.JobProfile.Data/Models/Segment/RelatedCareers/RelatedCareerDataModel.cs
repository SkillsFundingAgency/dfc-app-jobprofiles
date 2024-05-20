using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models.RelatedCareersModels
{
    public class RelatedCareerDataModel
    {
        public string RoutePrefix => "/job-profiles";

        public Guid Id { get; set; }

        [Required]
        public string ProfileLink { get; set; }

        [Required]
        public string Title { get; set; }

    }
}
