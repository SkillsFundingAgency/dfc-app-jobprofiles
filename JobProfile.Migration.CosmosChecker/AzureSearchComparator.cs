using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Azure;
using Azure.Search.Documents;

using static JobProfile.Migration.CosmosChecker.Helpers;

namespace JobProfile.Migration.CosmosChecker
{
    public class AzureSearchComparator
    {
        private const string errorFileName = "AzSearchFailureLog.txt";
        private static readonly SearchClient searchClientOld = new SearchClient(
        new Uri("https://dfc-beta-dev-01-search.search.windows.net/"),
        "dfc-digital-jobprofiles-r698",
        new AzureKeyCredential("asd"));

        //// DEV
        //private static readonly SearchClient searchClientNew = new SearchClient(
        //new Uri("https://dfc-beta-dev-01-search.search.windows.net/"),
        //"dfc-digital-jobprofiles-stax",
        //new AzureKeyCredential("67FE0C919175B0F29B297CEE16F9794B"));

        // SIT
        private static readonly SearchClient searchClientNew = new SearchClient(
        new Uri("https://dfc-beta-sit-01-search.search.windows.net/"),
        "dfc-digital-jobprofiles-stax",
        new AzureKeyCredential("asdsa"));


        public async Task CompareAzureCache()
        {
            Console.WriteLine("Starting Azure search index comparison...");
            int total = 0, matching = 0;
            var existingProfiles = await GetAllAsync(searchClientOld);

            var newProfiles = await GetAllAsync(searchClientNew);

            foreach (var profile in existingProfiles)
            {
                total++;
                var newProfile = newProfiles.FirstOrDefault(p => p.Title.Equals(profile.Title, StringComparison.OrdinalIgnoreCase));
                (bool doesMatch, string property) = DoesMatch(newProfile, profile);
                if (doesMatch)
                {
                    matching++;
                    //Console.WriteLine($"{total}: SUCCESS: Azure index matched for profile: {profile.Title}");
                }
                else
                {
                    await File.AppendAllTextAsync(errorFileName, $"FAILED: Cosmos document does not match for profile: {profile.Title} \r\n {property}.");
                    await File.AppendAllLinesAsync(errorFileName, new[] { "", "*********************************************", "" });
                    Console.WriteLine("--------------------");
                    Console.WriteLine($"{total}: FAILED: Azure index does not match for profile: {profile.Title} - {property}");
                }

            }

            Console.WriteLine($"Finished Azure search index comparison. Total profiles: {total}. Matching profiles: {matching}");
        }

        private (bool doesMatch, string property) DoesMatch(JobProfileIndex newProfile, JobProfileIndex oldProfile)
        {
            if (newProfile is null)
                return (false, "newProfile");
            //if (AreEqualOrNull(newProfile.IdentityField, oldProfile.IdentityField) is false)
            //    return (false, nameof(JobProfileIndex.IdentityField));
            if (AreEqualOrNull(newProfile.SocCode, oldProfile.SocCode) is false)
                return (false, Difference(nameof(JobProfileIndex.SocCode), oldProfile.SocCode, newProfile.SocCode));

            if (AreEqualOrNull(newProfile.Title, oldProfile.Title) is false)
                return (false, Difference(nameof(JobProfileIndex.Title), oldProfile.Title, newProfile.Title));

            //if (AreEqualOrNull(newProfile.AlternativeTitle, oldProfile.AlternativeTitle) is false)
            // TODO: temp check, remove later and uncomment the line above.
            if (AreEqualOrNull(string.Join(", ", newProfile.AlternativeTitle), string.Join(", ", oldProfile.AlternativeTitle)) is false)
                return (false, Difference(nameof(JobProfileIndex.AlternativeTitle), oldProfile.AlternativeTitle, newProfile.AlternativeTitle));

            if (AreEqualOrNull(newProfile.TitleAsKeyword, oldProfile.TitleAsKeyword) is false)
                return (false, Difference(nameof(JobProfileIndex.TitleAsKeyword), oldProfile.TitleAsKeyword, newProfile.TitleAsKeyword));

            if (AreEqualOrNull(newProfile.Overview, oldProfile.Overview) is false)
                return (false, Difference(nameof(JobProfileIndex.Overview), oldProfile.Overview, newProfile.Overview));

            if (AreEqualOrNull(newProfile.SalaryStarter, oldProfile.SalaryStarter) is false)
                return (false, Difference(nameof(JobProfileIndex.SalaryStarter), oldProfile.SalaryStarter.ToString(), newProfile.SalaryStarter.ToString()));

            if (AreEqualOrNull(newProfile.SalaryExperienced, oldProfile.SalaryExperienced) is false)
                return (false, Difference(nameof(JobProfileIndex.SalaryExperienced), oldProfile.SalaryExperienced.ToString(), newProfile.SalaryExperienced.ToString()));

            if (AreEqualOrNull(newProfile.UrlName, oldProfile.UrlName) is false)
                return (false, Difference(nameof(JobProfileIndex.UrlName), oldProfile.UrlName, newProfile.UrlName));

            if (AreEqualOrNull(newProfile.CollegeRelevantSubjects.Clean(), oldProfile.CollegeRelevantSubjects.Clean()) is false)
                return (false, Difference(nameof(JobProfileIndex.CollegeRelevantSubjects), oldProfile.CollegeRelevantSubjects, newProfile.CollegeRelevantSubjects));

            if (AreEqualOrNull(newProfile.UniversityRelevantSubjects.Clean(), oldProfile.UniversityRelevantSubjects.Clean()) is false)
                return (false, Difference(nameof(JobProfileIndex.UniversityRelevantSubjects), oldProfile.UniversityRelevantSubjects, newProfile.UniversityRelevantSubjects));

            if (AreEqualOrNull(newProfile.ApprenticeshipRelevantSubjects.Clean(), oldProfile.ApprenticeshipRelevantSubjects.Clean()) is false)
                return (false, Difference(nameof(JobProfileIndex.ApprenticeshipRelevantSubjects), oldProfile.ApprenticeshipRelevantSubjects, newProfile.ApprenticeshipRelevantSubjects));

            if (AreEqualOrNull(newProfile.WYDDayToDayTasks.Clean(), oldProfile.WYDDayToDayTasks.Clean()) is false)
                return (false, Difference(nameof(JobProfileIndex.WYDDayToDayTasks), oldProfile.WYDDayToDayTasks, newProfile.WYDDayToDayTasks));

            if (AreEqualOrNull(newProfile.CareerPathAndProgression.Clean(), oldProfile.CareerPathAndProgression.Clean()) is false)
                return (false, Difference(nameof(JobProfileIndex.CareerPathAndProgression), oldProfile.CareerPathAndProgression, newProfile.CareerPathAndProgression));

            if (AreEqualOrNull(newProfile.MinimumHours, oldProfile.MinimumHours) is false)
                return (false, Difference(nameof(JobProfileIndex.MinimumHours), oldProfile.MinimumHours.ToString(), newProfile.MinimumHours.ToString()));

            if (AreEqualOrNull(newProfile.MaximumHours, oldProfile.MaximumHours) is false)
                return (false, Difference(nameof(JobProfileIndex.MaximumHours), oldProfile.MaximumHours.ToString(), newProfile.MaximumHours.ToString()));

            if (AreEqualOrNull(newProfile.Rank, oldProfile.Rank) is false)
                return (false, Difference(nameof(JobProfileIndex.Rank), oldProfile.Rank.ToString(), newProfile.Rank.ToString()));

            if (AreEqualOrNull(newProfile.JobProfileCategories, oldProfile.JobProfileCategories) is false)
                return (false, Difference(nameof(JobProfileIndex.JobProfileCategories), oldProfile.JobProfileCategories, newProfile.JobProfileCategories));

            if (AreEqualOrNull(newProfile.JobProfileSpecialism, oldProfile.JobProfileSpecialism) is false)
                return (false, Difference(nameof(JobProfileIndex.JobProfileSpecialism), oldProfile.JobProfileSpecialism, newProfile.JobProfileSpecialism));

            if (AreEqualOrNull(newProfile.HiddenAlternativeTitle, oldProfile.HiddenAlternativeTitle) is false)
                return (false, Difference(nameof(JobProfileIndex.HiddenAlternativeTitle), oldProfile.HiddenAlternativeTitle, newProfile.HiddenAlternativeTitle));

            if (AreEqualOrNull(newProfile.JobProfileCategoriesWithUrl, oldProfile.JobProfileCategoriesWithUrl) is false)
                return (false, Difference(nameof(JobProfileIndex.JobProfileCategoriesWithUrl), oldProfile.JobProfileCategoriesWithUrl, newProfile.JobProfileCategoriesWithUrl));

            if (AreEqualOrNull(newProfile.JobProfileCategoryUrls, oldProfile.JobProfileCategoryUrls) is false)
                return (false, Difference(nameof(JobProfileIndex.JobProfileCategoryUrls), oldProfile.JobProfileCategoryUrls, newProfile.JobProfileCategoryUrls));

            if (AreEqualOrNull(newProfile.Skills, oldProfile.Skills) is false)
                return (false, Difference(nameof(JobProfileIndex.Skills), oldProfile.Skills, newProfile.Skills));

            if (AreEqualOrNull(newProfile.WorkingPattern, oldProfile.WorkingPattern) is false)
                return (false, Difference(nameof(JobProfileIndex.WorkingPattern), oldProfile.WorkingPattern, newProfile.WorkingPattern));

            if (AreEqualOrNull(newProfile.WorkingPatternDetails, oldProfile.WorkingPatternDetails) is false)
                return (false, Difference(nameof(JobProfileIndex.WorkingPatternDetails), oldProfile.WorkingPatternDetails, newProfile.WorkingPatternDetails));

            if (AreEqualOrNull(newProfile.WorkingHoursDetails, oldProfile.WorkingHoursDetails) is false)
                return (false, Difference(nameof(JobProfileIndex.WorkingHoursDetails), oldProfile.WorkingHoursDetails, newProfile.WorkingHoursDetails));

            return (true, string.Empty);
        }

        private async Task<IEnumerable<JobProfileIndex>> GetAllAsync(SearchClient azureSearchClient)
        {
            var searchResult = await azureSearchClient.SearchAsync<JobProfileIndex>("*", new SearchOptions { Size = 1000 });
            var results = searchResult.Value.GetResults();
            return results.Select(r => r.Document);
        }
    }
}
