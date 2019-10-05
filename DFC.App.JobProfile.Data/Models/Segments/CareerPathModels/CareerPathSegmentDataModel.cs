using DFC.App.JobProfile.Data.Contracts;
using System;

namespace DFC.App.JobProfile.Data.Models.Segments.CareerPathModels
{
    public class CareerPathSegmentDataModel : ISegmentDataModel
    {
        public const string SegmentName = "CareerPath";

        public DateTime? LastReviewed { get; set; }

        public string Markup { get; set; }
    }
}
