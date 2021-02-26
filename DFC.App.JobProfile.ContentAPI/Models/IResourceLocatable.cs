using System;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public interface IResourceLocatable
    {
        Uri Uri { get; set; }
    }
}
