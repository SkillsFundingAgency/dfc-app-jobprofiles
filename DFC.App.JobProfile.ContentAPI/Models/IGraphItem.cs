using System;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public interface IGraphItem
    {
        Uri Uri { get; }

        string ContentType { get; }

        string Title { get; }
    }
}
