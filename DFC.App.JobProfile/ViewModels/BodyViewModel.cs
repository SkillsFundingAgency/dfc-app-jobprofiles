// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
using DFC.App.JobProfile.Data.Models;

namespace DFC.App.JobProfile.ViewModels
{
    public class BodyViewModel
    {
        //public string CanonicalName { get; set; }

        //public IList<object> Segments { get; set; }

        //public string SmartSurveyJP { get; set; }

        public JobProfileWhatYoullDo WhatYoullDoSegment { get; set; }

        public JobProfileCareerPath CareerPathSegment { get; set; }

        public JobProfileHowToBecome HowToBecomeSegment { get; set; }

        public JobProfileWhatItTakes WhatItTakesSegment { get; set; }

        public CurrentOpportunities CurrentOpportunities { get; set; }
    }
}