using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class EducationalRoute
    {
        public string Topic { get; set; }

        public string RelevantSubjects { get; set; }

        public string FurtherInformation { get; set; }

        public EducationalRouteItem RequirementsAndReading { get; set; }
    }
}
