using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class ApiEducationalRouteItem
    {
        public string Preface { get; set; } = "You'll usually need:";

        public IReadOnlyCollection<string> Requirements { get; set; }

        public IReadOnlyCollection<ApiAnchor> FurtherReading { get; set; }
    }
}
