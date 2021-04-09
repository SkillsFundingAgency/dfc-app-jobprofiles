using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class ApiEducationalRoute
    {
        public string Topic { get; set; }

        public string RelevantSubjects { get; set; }

        public string FurtherInformation { get; set; }

        public ApiEducationalRouteItem RequirementsAndReading { get; set; }
    }
}
