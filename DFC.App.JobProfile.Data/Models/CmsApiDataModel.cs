using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class CmsApiDataModel : ICmsApiDataModel
    {
        public ContentLinksModel ContentLinks { get; set; }

        public IList<BaseContentItemModel> ContentItems { get; set; }
    }
}
