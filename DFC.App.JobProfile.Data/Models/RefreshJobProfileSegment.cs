using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class RefreshJobProfileSegment : BaseJobProfile
    {
        [Required]
        public JobProfileSegment Segment { get; set; }
    }
}