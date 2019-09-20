using DFC.App.JobProfile.Data.Enums;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.HowToBecomeDataModels
{
    public partial class HowToBecomeSegmentModel : BaseSegmentModel
    {
        public string Title { get; set; }

        public TitlePrefix TitlePrefix { get; set; }

        public string EntryRouteSummary { get; set; }

        public EntryRoutes EntryRoutes { get; set; }

        public MoreInformation MoreInformation { get; set; }

        public IEnumerable<string> Registrations { get; set; }
    }
}