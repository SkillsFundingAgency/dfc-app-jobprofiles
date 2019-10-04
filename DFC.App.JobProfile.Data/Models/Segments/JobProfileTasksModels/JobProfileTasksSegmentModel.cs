namespace DFC.App.JobProfile.Data.Models.Segments.JobProfileTasksModels
{
    public class JobProfileTasksSegmentModel : BaseSegmentModel
    {
        public const string SegmentName = "WhatYouWillDo";

        public JobProfileTasksDataSegmentModel Data { get; set; }
    }
}