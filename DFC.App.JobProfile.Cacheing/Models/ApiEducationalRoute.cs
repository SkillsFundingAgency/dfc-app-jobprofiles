namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ApiEducationalRoute
    {
        public string Topic { get; set; }

        public string RelevantSubjects { get; set; }

        public string FurtherInformation { get; set; }

        public ApiEducationalRouteItem RequirementsAndReading { get; set; }
    }
}
