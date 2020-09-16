using dfc_content_pkg_netcore.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfilesContentItemModel : BaseContentItemModel
    {
        public Uri Url { get; set; }

        public string Description { get; set; }

        public string FurtherInfo { get; set; }

        public string RelevantSubjects { get; set; }

        public string ContentType { get; set; }

        public IList<JobProfilesContentItemModel> ContentItems { get; set; } = new List<JobProfilesContentItemModel>();

        [JsonIgnore]
        private ContentLinksModel PrivateLinksModel { get; set; }
    }
}
