using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Models
{
    public class CreateOrUpdateJobProfileModel
    {
        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public bool RefreshAllSegments { get; set; }

        public bool RefreshOverviewBannerSegment { get; set; }

        public bool RefreshCurrentOpportunitiesSegment { get; set; }

        public bool RefreshRelatedCareersSegment { get; set; }

        public bool RefreshCareerPathSegment { get; set; }

        public bool RefreshHowToBecomeSegment { get; set; }

        public bool RefreshWhatItTakesSegment { get; set; }
    }
}