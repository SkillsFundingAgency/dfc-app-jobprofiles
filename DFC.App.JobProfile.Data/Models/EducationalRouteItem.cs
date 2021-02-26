using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class EducationalRouteItem
    {
        public string Preamble { get; set; }

        public IReadOnlyCollection<string> Requirements { get; set; }

        public IReadOnlyCollection<Anchor> FurtherReading { get; set; }
    }
}
