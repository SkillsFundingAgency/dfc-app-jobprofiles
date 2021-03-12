using Newtonsoft.Json;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class SegmentProvider
    {
        [JsonProperty("Opportunities")]
        public IReadOnlyCollection<SegmentCourse> Courses { get; set; }
    }
}
