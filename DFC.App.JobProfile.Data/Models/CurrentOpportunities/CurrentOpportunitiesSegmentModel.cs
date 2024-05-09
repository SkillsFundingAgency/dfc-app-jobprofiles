using DFC.App.JobProfile.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Models.CurrentOpportunities
{
    public class CurrentOpportunitiesSegmentModel
    {
        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        [Required]
        public long SequenceNumber { get; set; }

        public CurrentOpportunitiesSegmentDataModel Data { get; set; }
    }
}
