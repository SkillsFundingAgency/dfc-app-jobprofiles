using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ApiEducationalRouteItem
    {
        public string Preamble { get; set; }

        public IReadOnlyCollection<string> Requirements { get; set; }

        public IReadOnlyCollection<ApiAnchor> FurtherReading { get; set; }
    }
}
