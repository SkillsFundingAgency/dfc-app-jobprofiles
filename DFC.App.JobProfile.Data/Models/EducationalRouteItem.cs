using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class EducationalRouteItem
    {
        public string Preface { get; set; }

        public IReadOnlyCollection<string> Requirements { get; set; }

        public IReadOnlyCollection<Anchor> FurtherReading { get; set; }
    }
}
