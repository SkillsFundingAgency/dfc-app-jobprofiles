using DFC.App.JobProfile.Data.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.JobProfileTasksModels
{
    public class JobProfileTasksSegmentDataModel : ISegmentDataModel
    {
        public const string SegmentName = "WhatYouWillDo";

        public DateTime LastReviewed { get; set; }

        public string Introduction { get; set; }

        public string Tasks { get; set; }

        public IEnumerable<JobProfileTasksDataLocationSegmentModel> Locations { get; set; }

        public IEnumerable<JobProfileTasksDataEnvironmentSegmentModel> Environments { get; set; }

        public IEnumerable<JobProfileTasksDataUniformSegmentModel> Uniforms { get; set; }
    }
}
