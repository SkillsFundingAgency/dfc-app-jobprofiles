﻿// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileWhatItTakes
    {
        //public IReadOnlyCollection<ContentApiBranchElement> OtherRequirement { get; set; }
        //public IReadOnlyCollection<ContentApiBranchElement> Restrictions { get; set; }
        public IReadOnlyCollection<string> Skills { get; set; }
    }
}