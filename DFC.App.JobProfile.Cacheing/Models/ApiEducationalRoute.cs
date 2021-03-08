﻿namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ApiEducationalRoute
    {
        public string FurtherInformation { get; set; }

        public string RelevantSubjects { get; set; }

        public ApiEducationalRouteItem MoreInformation { get; set; }
    }
}