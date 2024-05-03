using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Criterion;

namespace DFC.App.JobProfile.Data.Models.SkillsModels
{
    public class JobProfileSkillSegmentDataModel
    {
        public const string SegmentName = "WhatItTakes";

        public DateTime LastReviewed { get; set; }

        public string OtherRequirements { get; set; }

        public string DigitalSkill { get; set; }

        public IList<Skills> Skills { get; set; }

        public List<Restriction> Restrictions { get; set; }
    }
}
