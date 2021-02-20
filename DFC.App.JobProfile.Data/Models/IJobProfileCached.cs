using DFC.Compui.Cosmos.Contracts;
using System;

namespace DFC.App.JobProfile.Data.Models
{
    public interface IJobProfileCached :
        IContentPageModel
    {
        Uri Uri { get; }

        bool IsDefaultForPageLocation { get; }
    }
}
