using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;

namespace DFC.App.JobProfile.ViewModels
{
    public class BodyViewModel
    {
        public string CanonicalName { get; set; }

        public IList<SegmentModel> Segments { get; set; }

        public string SmartSurveyJP { get; set; }

        public StaticContentItemModel SpeakToAnAdviser { get; set; }
        /// <summary>
        /// Gets or sets the social proof video when one is enabled for the job profile.
        /// </summary>
        /// <value>
        /// A <see cref="SocialProofVideo"/> when present; otherwise, a value of <c>null</c>.
        /// </value>
        public SocialProofVideo Video { get; set; }
    }
}