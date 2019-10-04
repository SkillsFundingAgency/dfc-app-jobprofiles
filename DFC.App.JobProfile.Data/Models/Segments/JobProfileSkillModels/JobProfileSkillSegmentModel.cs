using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.JobProfileSkillModels
{
    public class JobProfileSkillSegmentModel : BaseSegmentModel
    {
        public const string SegmentName = "WhatItTakes";

        public JobProfileSkillSegmentDataModel Data { get; set; }
    }
}
