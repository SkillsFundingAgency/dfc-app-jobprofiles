using Newtonsoft.Json;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class SegmentData
    {
        [JsonProperty("Apprenticeships")]
        public SegmentApprenticeship Apprenticeship { get; set; }

        [JsonProperty("Courses")]
        public SegmentProvider Provider { get; set; }
    }
}
