using System;

namespace DFC.App.JobProfile.Data.Models.Segment.Overview
{
    public class SocData
    {
        public Guid Id { get; set; }

        public string SocCode { get; set; }

        public string Soc2020 { get; set; }

        public string Soc2020Extension { get; set; }

        public string Description { get; set; }

        public string ONetOccupationalCode { get; set; }

        public string UrlName { get; set; }
    }
}