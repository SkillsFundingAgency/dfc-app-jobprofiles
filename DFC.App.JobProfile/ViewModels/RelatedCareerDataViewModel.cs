using System.ComponentModel.DataAnnotations;
using System;

namespace DFC.App.JobProfile.ViewModels
{
    public class RelatedCareerDataViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string ProfileLink { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
