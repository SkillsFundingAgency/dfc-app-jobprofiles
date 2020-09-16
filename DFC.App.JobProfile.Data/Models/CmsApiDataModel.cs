

using dfc_content_pkg_netcore.contracts;
using dfc_content_pkg_netcore.models;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class CmsApiDataModel : ICmsApiDataModel
    {
        public ContentLinksModel ContentLinks { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<BaseContentItemModel> ContentItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
