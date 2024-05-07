using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class ApprenticeshipVacancyDetails
    {
        public DateTime ClosingDate { get; set; }

        public string Description { get; set; }

        public string EmployerName { get; set; }

        public double HoursPerWeek { get; set; }

        public bool IsDisabilityConfident { get; set; }

        public bool IsNationalVacancy { get; set; }

        public int NumberOfPositions { get; set; }

        public string PostedDate { get; set; }

        public string ProviderName { get; set; }

        public DateTime StartDate { get; set; }

        public string Title { get; set; }

        public string Ukprn { get; set; }

        public int VacancyReference { get; set; }

        public Uri VacancyUrl { get; set; }

        public ApprenticeshipCourse Course { get; set; }

        public Wage Wage { get; set; }

        public ApprenticeshipLocation Location { get; set; }

        public AddressLocation Address { get; set; }

        public double? Distance { get; set; }

        public string EmployerContactPhone { get; set; }

        public string EmployerContactName { get; set; }

        public string EmployerContactEmail { get; set; }

        public Uri EmployerWebsiteUrl { get; set; }

        public string ApprenticeshipLevel { get; set; }

        public string ExpectedDuration { get; set; }

        public string TrainingDescription { get; set; }

        public string FullDescription { get; set; }

        public string OutcomeDescription { get; set; }

        public string EmployerDescription { get; set; }

        public List<string> Skills { get; set; }

        public List<Qualification> Qualifications { get; set; }

        public string ThingsToConsider { get; set; }
    }
}
