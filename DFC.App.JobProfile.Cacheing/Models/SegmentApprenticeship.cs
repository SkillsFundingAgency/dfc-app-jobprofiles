using Newtonsoft.Json;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class SegmentApprenticeship
    {
        [JsonProperty("Vacancies")]
        public IReadOnlyCollection<SegmentVacancy> Vacancies { get; set; }
    }
}
