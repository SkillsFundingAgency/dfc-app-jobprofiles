using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class SegmentData
    {
        [JsonProperty("Apprenticeships")]
        public SegmentApprenticeship Apprenticeship { get; set; }

        [JsonProperty("Courses")]
        public SegmentProvider Provider { get; set; }
    }
}
