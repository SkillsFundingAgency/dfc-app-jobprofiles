using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class JobProfileHowToBecome
    {
        public EducationalRoute UniversityRoute { get; set; }

        public EducationalRoute CollegeRoute { get; set; }

        public EducationalRoute ApprenticeshipRoute { get; set; }

        public GeneralRoute WorkRoute { get; set; }

        public GeneralRoute DirectRoute { get; set; }

        public GeneralRoute VolunteeringRoute { get; set; }

        public GeneralRoute OtherRoute { get; set; }

        public HowToBecomeMoreInformation MoreInformation { get; set; }
    }
}
