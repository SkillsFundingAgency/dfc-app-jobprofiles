using System.Collections.Generic;

namespace JobProfile.Migration.CosmosChecker
{
    internal class JobProfileIndex
    {
        public string IdentityField { get; set; }

        public string SocCode { get; set; }

        public string Title { get; set; }

        public string TitleAsKeyword { get; set; }

        public IEnumerable<string> AlternativeTitle { get; set; }

        public IEnumerable<string> AltTitleAsKeywords { get; set; }

        public string Overview { get; set; }

        public double SalaryStarter { get; set; }

        public double SalaryExperienced { get; set; }

        public string UrlName { get; set; }

        public IEnumerable<string> JobProfileCategories { get; set; }

        public IEnumerable<string> JobProfileSpecialism { get; set; }

        public IEnumerable<string> HiddenAlternativeTitle { get; set; }

        public IEnumerable<string> JobProfileCategoriesWithUrl { get; set; }

        public IEnumerable<string> JobProfileCategoryUrls { get; set; }

        public IEnumerable<string> Interests { get; set; }

        public IEnumerable<string> Enablers { get; set; }

        public IEnumerable<string> EntryQualifications { get; set; }

        public IEnumerable<string> TrainingRoutes { get; set; }

        public IEnumerable<string> PreferredTaskTypes { get; set; }

        public IEnumerable<string> JobAreas { get; set; }

        public IEnumerable<string> Skills { get; set; }

        public double EntryQualificationLowestLevel { get; set; }

        public string CollegeRelevantSubjects { get; set; }

        public string UniversityRelevantSubjects { get; set; }

        public string ApprenticeshipRelevantSubjects { get; set; }

        public string WYDDayToDayTasks { get; set; }

        public string CareerPathAndProgression { get; set; }

        public IEnumerable<string> WorkingPattern { get; set; }

        public IEnumerable<string> WorkingPatternDetails { get; set; }

        public IEnumerable<string> WorkingHoursDetails { get; set; }

        public double MinimumHours { get; set; }

        public double MaximumHours { get; set; }

        public int Rank { get; set; }
    }
}