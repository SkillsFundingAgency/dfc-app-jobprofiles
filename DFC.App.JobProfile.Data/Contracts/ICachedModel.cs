using System;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ICachedModel
    {
        string? Title { get; set; }

        Uri? Url { get; set; }
    }
}
