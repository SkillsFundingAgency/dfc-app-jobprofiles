using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ApiEducationalRouteItem
    {
        public string Preface { get; set; } = "You may need:";

        public IReadOnlyCollection<string> Requirements { get; set; }

        public IReadOnlyCollection<ApiAnchor> FurtherReading { get; set; }
    }
}
