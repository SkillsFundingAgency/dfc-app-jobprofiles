namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ContentApiHowToBecome
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
