using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    [ExcludeFromCodeCoverage]
    public static class UriExtra
    {
        public static Uri Empty => new Uri("about:nothing");
    }
}
