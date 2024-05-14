using System;
using System.Xml.Linq;

namespace DFC.App.JobProfile.Data.Models.Overview
{
    public class WorkingHoursDetail
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }
    }
}