using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;

namespace DFC.App.JobProfile.ViewModels
{
    public class BodyViewModel
    {
        public string CanonicalName { get; set; }

        public IList<SegmentModel> Segments { get; set; }

        public string SmartSurveyJP { get; set; }

    }
}
