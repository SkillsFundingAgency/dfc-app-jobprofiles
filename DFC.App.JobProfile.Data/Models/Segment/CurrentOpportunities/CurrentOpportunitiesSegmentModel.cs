using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models.CurrentOpportunities
{
    public class CurrentOpportunitiesSegmentModel
    {
        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        [Required]
        public long SequenceNumber { get; set; }

        public CurrentOpportunitiesSegmentDataModel Data { get; set; }
    }
}
