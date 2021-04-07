using System;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public interface IGraphSummaryItem :
        IResourceLocatable
    {
        string CanonicalName { get; }

        DateTime ModifiedDateTime { get; }
    }
}