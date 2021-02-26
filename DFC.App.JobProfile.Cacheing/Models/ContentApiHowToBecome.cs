using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public class ApiAnchor
    {
        public string Text { get; set; }

        public string Link { get; set; }
    }

    public class ApiEducationalRoute
    {
        public string FurtherInformation { get; set; }

        public string RelevantSubjects { get; set; }

        public ApiEducationalRouteItem MoreInformation { get; set; }
    }

    public class ApiEducationalRouteItem
    {
        public string Preamble { get; set; }

        public IReadOnlyCollection<string> Requirements { get; set; }

        public IReadOnlyCollection<ApiAnchor> FurtherReading { get; set; }
    }

    public class ContentApiHowToBecome
    {
        public string Title { get; set; }

        public ApiEducationalRoute UniversityRoute { get; set; }

        public ApiEducationalRoute CollegeRoute { get; set; }

        public ApiEducationalRoute ApprenticeshipRoute { get; set; }

        public string WorkRoute { get; set; }

        public string DirectRoute { get; set; }

        public string VolunteeringRoute { get; set; }

        public string OtherRoute { get; set; }

        public string Registration { get; set; }

        public ContentApiHowToBecomeMoreInformation MoreInformation { get; set; }
    }
}
