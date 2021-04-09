using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class SegmentApprenticeship
    {
        [JsonProperty("Vacancies")]
        public IReadOnlyCollection<SegmentVacancy> Vacancies { get; set; }
    }
}
