using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class ContentApiHowToBecome
    {
        public ApiEducationalRoute UniversityRoute { get; set; }

        public ApiEducationalRoute CollegeRoute { get; set; }

        public ApiEducationalRoute ApprenticeshipRoute { get; set; }

        public ApiGeneralRoute WorkRoute { get; set; }

        public ApiGeneralRoute DirectRoute { get; set; }

        public ApiGeneralRoute VolunteeringRoute { get; set; }

        public ApiGeneralRoute OtherRoute { get; set; }

        public ContentApiHowToBecomeMoreInformation MoreInformation { get; set; }
    }
}
