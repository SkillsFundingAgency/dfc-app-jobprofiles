using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Apprenticeships

{
    public class ApprenticeshipVacancySummaryResponse
    {
        public int Total { get; set; }

        public int TotalFiltered { get; set; }

        public double TotalPages { get; set; }

        public IEnumerable<ApprenticeshipVacancySummary> Vacancies { get; set; }
    }
}
