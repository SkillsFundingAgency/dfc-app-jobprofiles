using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models.Segment.CurrentOpportunities
{
    public class CurrentOpportunitiesSegmentModel
    {
        [Required]
        public string CanonicalName { get; set; }

        public CurrentOpportunitiesSegmentDataModel Data { get; set; }
    }
}
