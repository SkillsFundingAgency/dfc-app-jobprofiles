using System.ComponentModel.DataAnnotations;
using System;

namespace DFC.App.JobProfile.ViewModels.Segment.Skills
{
    public class BodyViewModel
    {
        [Display(Name = "Document Id")]
        public Guid? DocumentId { get; set; }

        [Display(Name = "Canonical Name")]
        public string CanonicalName { get; set; }

        public BodyDataViewModel Data { get; set; }
    }
}
