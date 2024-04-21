using DFC.App.JobProfile.Data.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.HowToBecome
{
    public class CommonRoutes
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RouteName RouteName { get; set; }

        public string Subject { get; set; }

        public string FurtherInformation { get; set; }

        public string EntryRequirementPreface { get; set; }

        public List<EntryRequirement> EntryRequirements { get; set; }

        public List<AdditionalInformation> AdditionalInformation { get; set; }
    }
}