using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.ViewModels
{
    public class ReturnToAssessmentViewModel
    {
        [Required(ErrorMessage = "Enter your reference")]
        [Display(Name = "Enter your reference")]
        public string ReferenceId { get; set; }

        public bool HasError { get; set; }

        public string ActionUrl { get; set; }
    }
}
