using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.HowToBecomeModels
{
    public class EntryRoutes
    {
        public IEnumerable<CommonRoutes> CommonRoutes { get; set; }

        public string Work { get; set; }

        public string Volunteering { get; set; }

        public string DirectApplication { get; set; }

        public string OtherRoutes { get; set; }
    }
}