using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.ViewSupport.ViewModels
{
    public class BodyViewModel
    {
        public string PageLocation { get; set; }

        public string SmartSurveyJP { get; set; }

        public string SpeakToAnAdvisor { get; set; }

        public string SkillsAssessment { get; set; }

        public string NotWhatYoureLookingFor { get; set; }

        public JobProfileWhatYouWillDo WhatYouWillDoSegment { get; set; }

        public JobProfileCareerPath CareerPathSegment { get; set; }

        public JobProfileHowToBecome HowToBecomeSegment { get; set; }

        public JobProfileWhatItTakes WhatItTakesSegment { get; set; }

        public CurrentOpportunities CurrentOpportunities { get; set; }

        public IReadOnlyCollection<Anchor> RelatedCareers { get; set; }

        public string GetFeedbackSurveyLink() =>
            $"{SmartSurveyJP}{PageLocation}";

        public bool HasCurrentOpportunities() =>
            CurrentOpportunities?.HasItemsToDisplay() ?? false;

        public bool HasRelatedCareers() =>
            RelatedCareers?.Any() ?? false;
    }
}