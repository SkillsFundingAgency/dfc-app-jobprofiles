using System.Collections.Generic;
using DFC.App.JobProfile.Data.Models.Segment.CurrentOpportunities;
using Newtonsoft.Json;

namespace DFC.App.JobProfile.AVService.UnitTests.AVServiceTests
{
    public static class AVAPIDummyResponses
    {
        public static string GetDummyApprenticeshipVacancySummaryResponse(int currentPage, int totalMatches, int nunmberToReturn, int pageSize, int diffrentProvidersPage)
        {
            var r = new ApprenticeshipVacancySummaryResponse
            {
                Total = totalMatches,
                TotalPages = totalMatches / pageSize,
                TotalFiltered = nunmberToReturn,
            };

            var recordsToReturn = new List<ApprenticeshipVacancySummary>();

            for (int ii = 0; ii < nunmberToReturn; ii++)
            {
                recordsToReturn.Add(new ApprenticeshipVacancySummary()
                {
                    VacancyReference = ii,
                    Title = $"Title {ii}",
                    ProviderName = $"TrainingProviderName {((currentPage == diffrentProvidersPage) ? ii : currentPage)}",
                });
            }

            r.Vacancies = recordsToReturn.ToArray();

            return JsonConvert.SerializeObject(r);
        }

        public static string GetDummyApprenticeshipVacancyDetailsResponse()
        {
            var r = new ApprenticeshipVacancyDetails()
            {
                VacancyReference = 123,
            };

            return JsonConvert.SerializeObject(r);
        }
    }
}
