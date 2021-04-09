using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class JobProfileWhatItTakes
    {
        public string Preface { get; set; }

        public IReadOnlyCollection<string> Restrictions { get; set; }

        public IReadOnlyCollection<string> OtherRequirements { get; set; }

        public IReadOnlyCollection<string> Skills { get; set; }
    }
}
