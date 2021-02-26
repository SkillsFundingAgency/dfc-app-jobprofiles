using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileCachedHowToBecome
    {
        public string Title { get; set; }

        public EducationalRoute UniversityRoute { get; set; }

        public EducationalRoute CollegeRoute { get; set; }

        public EducationalRoute ApprenticeshipRoute { get; set; }

        public string WorkRoute { get; set; }

        public string DirectRoute { get; set; }

        public string VolunteeringRoute { get; set; }

        public string OtherRoute { get; set; }

        public string Registration { get; set; }

        public JobProfileCachedHowToBecomeMoreInformation MoreInformation { get; set; }
    }
}
