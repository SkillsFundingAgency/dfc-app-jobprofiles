using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public interface IContentApiBranchElement :
        IContentPageModel
    {
        Uri Uri { get; }

        Guid ItemID { get; }

        IEnumerable<IContentApiBranchElement> ContentItems { get; }
    }
}