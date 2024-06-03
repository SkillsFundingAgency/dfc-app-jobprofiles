using System;

namespace DFC.App.JobProfile.Data.Models.Segment.CurrentOpportunities
{
    public class ApprenticeshipVacancySummary
    {
        public DateTime ClosingDate { get; set; }

        public string Description { get; set; }

        public string EmployerName { get; set; }

        public double HoursPerWeek { get; set; }

        public bool IsDisabilityConfident { get; set; }

        public bool IsNationalVacancy { get; set; }

        public int NumberOfPositions { get; set; }

        public DateTime PostedDate { get; set; }

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

        public string EmployerContactPhone { get; set; }

        public string EmployerContactName { get; set; }

        public string EmployerContactEmail { get; set; }

        public Uri EmployerWebsiteUrl { get; set; }

        public string ApprenticeshipLevel { get; set; }

        public string ExpectedDuration { get; set; }
    }
}
