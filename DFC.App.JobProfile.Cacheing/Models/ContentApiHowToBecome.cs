﻿namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ContentApiHowToBecome
    {
        public string Title { get; set; }

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
