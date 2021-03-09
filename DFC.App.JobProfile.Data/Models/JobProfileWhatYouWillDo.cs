using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileWhatYouWillDo
    {
        public string DayToDayTasks { get; set; }

        public IReadOnlyCollection<string> WorkingEnvironment { get; set; }

        public IReadOnlyCollection<string> WorkingLocation { get; set; }

        public IReadOnlyCollection<string> WorkingUniform { get; set; }
    }
}
