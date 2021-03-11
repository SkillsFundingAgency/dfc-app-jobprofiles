using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileWhatItTakes
    {
        public string Preface { get; set; }

        public IReadOnlyCollection<string> Restrictions { get; set; }

        public IReadOnlyCollection<string> OtherRequirements { get; set; }

        public IReadOnlyCollection<string> Skills { get; set; }
    }
}
