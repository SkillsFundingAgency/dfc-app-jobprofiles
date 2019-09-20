using DFC.App.JobProfile.Data.Enums;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.HowToBecomeDataModels
{
    public class CommonRoutes
    {
        public RouteName RouteName { get; set; }

        public string Subject { get; set; }

        public string FurtherInformation { get; set; }

        public string EntryRequirementPreface { get; set; }

        public IEnumerable<string> EntryRequirements { get; set; }

        public IEnumerable<AdditionalInformation> AdditionalInformation { get; set; }
    }
}