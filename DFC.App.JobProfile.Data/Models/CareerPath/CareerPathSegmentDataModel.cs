using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFC.App.JobProfile.Data.Contracts;
using Newtonsoft.Json;

namespace DFC.App.JobProfile.Data.Models.CareerPath
{
    public class CareerPathSegmentDataModel
    {
        public const string SegmentName = "CareerPathsAndProgression";

        public DateTime LastReviewed { get; set; }

        public string Markup { get; set; }
    }
}
