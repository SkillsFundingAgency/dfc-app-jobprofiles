using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Azure;
using Azure.Search.Documents;

using static JobProfile.Migration.CosmosChecker.Helpers;

namespace JobProfile.Migration.CosmosChecker
{
    public class AzureSearchComparator
    {
        private static readonly SearchClient searchClientPre = new SearchClient(
        new Uri("https://dfc-beta-dev-01-search.search.windows.net/"),
        "dfc-digital-jobprofiles-r698",
        new AzureKeyCredential("67FE0C919175B0F29B297CEE16F9794B"));

        //private static readonly SearchClient searchClientPost = new SearchClient(
        //    new Uri(""),
        //    "",
        //    new AzureKeyCredential(""));

        public async Task CompareAzureCache()
        {
            Console.WriteLine("Starting Azure search index comparison...");
            int total = 0, matching = 0;
            var existingProfiles = await GetAllAsync(searchClientPre);

            var newProfiles = await GetAllAsync(searchClientPre);

            foreach (var profile in existingProfiles)
            {
                total++;
                var newProfile = newProfiles.FirstOrDefault(p => p.IdentityField.Equals(profile.IdentityField));
                (bool doesMatch, string property) = DoesMatch(newProfile, profile);
                if (doesMatch)
                {
                    matching++;
                    Console.WriteLine($"{total}: SUCCESS: Azure index matched for profile: {profile.Title}");
                }
                else
                {
                    Console.WriteLine($"{total}: FAILED: Azure index does not match for profile: {profile.Title} - ({property}).");
                }

            }

            Console.WriteLine($"Finished Azure search index comparison. Total profiles: {total}. Matching profiles: {matching}");
        }

        private (bool doesMatch, string property) DoesMatch(JobProfileIndex newProfile, JobProfileIndex oldProfile)
        {
            if (AreEqualOrNull(newProfile.IdentityField, oldProfile.IdentityField) is false)
                return (false, nameof(JobProfileIndex.IdentityField));
            if (AreEqualOrNull(newProfile.SocCode, oldProfile.SocCode) is false)
                return (false, nameof(JobProfileIndex.SocCode));
            if (AreEqualOrNull(newProfile.Title, oldProfile.Title) is false)
                return (false, nameof(JobProfileIndex.Title));
            if (AreEqualOrNull(newProfile.TitleAsKeyword, oldProfile.TitleAsKeyword) is false)
                return (false, nameof(JobProfileIndex.TitleAsKeyword));

            if (AreEqualOrNull(newProfile.Overview, oldProfile.Overview) is false)
                return (false, nameof(JobProfileIndex.Overview));
            if (AreEqualOrNull(newProfile.SalaryStarter, oldProfile.SalaryStarter) is false)
                return (false, nameof(JobProfileIndex.SalaryStarter));
            if (AreEqualOrNull(newProfile.SalaryExperienced, oldProfile.SalaryExperienced) is false)
                return (false, nameof(JobProfileIndex.SalaryExperienced));
            if (AreEqualOrNull(newProfile.UrlName, oldProfile.UrlName) is false)
                return (false, nameof(JobProfileIndex.UrlName));

            if (AreEqualOrNull(newProfile.EntryQualificationLowestLevel, oldProfile.EntryQualificationLowestLevel) is false)
                return (false, nameof(JobProfileIndex.EntryQualificationLowestLevel));
            if (AreEqualOrNull(newProfile.CollegeRelevantSubjects, oldProfile.CollegeRelevantSubjects) is false)
                return (false, nameof(JobProfileIndex.CollegeRelevantSubjects));
            if (AreEqualOrNull(newProfile.UniversityRelevantSubjects, oldProfile.UniversityRelevantSubjects) is false)
                return (false, nameof(JobProfileIndex.UniversityRelevantSubjects));
            if (AreEqualOrNull(newProfile.ApprenticeshipRelevantSubjects, oldProfile.ApprenticeshipRelevantSubjects) is false)
                return (false, nameof(JobProfileIndex.ApprenticeshipRelevantSubjects));
            if (AreEqualOrNull(newProfile.WYDDayToDayTasks, oldProfile.WYDDayToDayTasks) is false)
                return (false, nameof(JobProfileIndex.WYDDayToDayTasks));
            if (AreEqualOrNull(newProfile.CareerPathAndProgression, oldProfile.CareerPathAndProgression) is false)
                return (false, nameof(JobProfileIndex.CareerPathAndProgression));

            if (AreEqualOrNull(newProfile.MinimumHours, oldProfile.MinimumHours) is false)
                return (false, nameof(JobProfileIndex.MinimumHours));
            if (AreEqualOrNull(newProfile.MaximumHours, oldProfile.MaximumHours) is false)
                return (false, nameof(JobProfileIndex.MaximumHours));
            if (AreEqualOrNull(newProfile.Rank, oldProfile.Rank) is false)
                return (false, nameof(JobProfileIndex.Rank));

            if (AreEqualOrNull(newProfile.AlternativeTitle, oldProfile.AlternativeTitle) is false)
                return (false, nameof(JobProfileIndex.AlternativeTitle));

            if (AreEqualOrNull(newProfile.JobProfileCategories, oldProfile.JobProfileCategories) is false)
                return (false, nameof(JobProfileIndex.JobProfileCategories));
            if (AreEqualOrNull(newProfile.JobProfileSpecialism, oldProfile.JobProfileSpecialism) is false)
                return (false, nameof(JobProfileIndex.JobProfileSpecialism));
            if (AreEqualOrNull(newProfile.HiddenAlternativeTitle, oldProfile.HiddenAlternativeTitle) is false)
                return (false, nameof(JobProfileIndex.HiddenAlternativeTitle));
            if (AreEqualOrNull(newProfile.JobProfileCategoriesWithUrl, oldProfile.JobProfileCategoriesWithUrl) is false)
                return (false, nameof(JobProfileIndex.JobProfileCategoriesWithUrl));
            if (AreEqualOrNull(newProfile.JobProfileCategoryUrls, oldProfile.JobProfileCategoryUrls) is false)
                return (false, nameof(JobProfileIndex.JobProfileCategoryUrls));
            if (AreEqualOrNull(newProfile.Interests, oldProfile.Interests) is false)
                return (false, nameof(JobProfileIndex.Interests));
            if (AreEqualOrNull(newProfile.Enablers, oldProfile.Enablers) is false)
                return (false, nameof(JobProfileIndex.Enablers));
            if (AreEqualOrNull(newProfile.EntryQualifications, oldProfile.EntryQualifications) is false)
                return (false, nameof(JobProfileIndex.EntryQualifications));
            if (AreEqualOrNull(newProfile.TrainingRoutes, oldProfile.TrainingRoutes) is false)
                return (false, nameof(JobProfileIndex.TrainingRoutes));
            if (AreEqualOrNull(newProfile.PreferredTaskTypes, oldProfile.PreferredTaskTypes) is false)
                return (false, nameof(JobProfileIndex.PreferredTaskTypes));
            if (AreEqualOrNull(newProfile.JobAreas, oldProfile.JobAreas) is false)
                return (false, nameof(JobProfileIndex.JobAreas));
            if (AreEqualOrNull(newProfile.Skills, oldProfile.Skills) is false)
                return (false, nameof(JobProfileIndex.Skills));

            if (AreEqualOrNull(newProfile.WorkingPattern, oldProfile.WorkingPattern) is false)
                return (false, nameof(JobProfileIndex.WorkingPattern));
            if (AreEqualOrNull(newProfile.WorkingPatternDetails, oldProfile.WorkingPatternDetails) is false)
                return (false, nameof(JobProfileIndex.WorkingPatternDetails));
            if (AreEqualOrNull(newProfile.WorkingHoursDetails, oldProfile.WorkingHoursDetails) is false)
                return (false, nameof(JobProfileIndex.WorkingHoursDetails));

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
