using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class CareerPathSegmentModel
    {
        [Display(Name = "Last Reviewed")]
        public DateTime LastReviewed { get; set; }

        public string Content { get; set; }
    }
}
