using DFC.App.JobProfile.Data.Models;

namespace DFC.App.JobProfile.ViewSupport.ViewModels
{
    public class BodyViewModel
    {
        public string SmartSurveyJP { get; set; }

        public string SpeakToAnAdvisor { get; set; }

        public string SkillsAssessment { get; set; }

        public string NotWhatYoureLookingFor { get; set; }

        public JobProfileWhatYouWillDo WhatYouWillDoSegment { get; set; }

        public JobProfileCareerPath CareerPathSegment { get; set; }

        public JobProfileHowToBecome HowToBecomeSegment { get; set; }

        public JobProfileWhatItTakes WhatItTakesSegment { get; set; }

        public CurrentOpportunities CurrentOpportunities { get; set; }
    }
}