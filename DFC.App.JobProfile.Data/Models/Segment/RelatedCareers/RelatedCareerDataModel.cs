using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
