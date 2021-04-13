using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class SegmentProvider
    {
        [JsonProperty("Opportunities")]
        public IReadOnlyCollection<SegmentCourse> Courses { get; set; }
    }
}
