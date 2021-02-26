// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public class ContentApiWhatItTakes
    {
        //public IReadOnlyCollection<ContentApiBranchElement> OtherRequirement { get; set; }

        //public IReadOnlyCollection<ContentApiBranchElement> Restrictions { get; set; }

        public IReadOnlyCollection<string> Skills { get; set; }
    }
}
