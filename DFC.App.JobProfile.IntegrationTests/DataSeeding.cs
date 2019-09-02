using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace DFC.App.JobProfile.IntegrationTests
{
    public static class DataSeeding
    {
        public static void SeedDefaultArticle(CustomWebApplicationFactory<Startup> factory, Guid articleGuid, string article)
        {
            const string url = "/pages";
            var helpPageModels = new List<JobProfileModel>()
            {
                new JobProfileModel()
                {
                    DocumentId = articleGuid,
                    CanonicalName = article,
                    BreadcrumbTitle = article,
                    IncludeInSitemap = true,
                    LastReviewed = DateTime.UtcNow,
                },
                new JobProfileModel()
                {
                    DocumentId = Guid.Parse("C16B389D-91AD-4F3D-2485-9F7EE953AFE4"),
                    CanonicalName = "in-sitemap",
                    BreadcrumbTitle = article,
                    IncludeInSitemap = true,
                    LastReviewed = DateTime.UtcNow,
                },
                new JobProfileModel()
                {
                    DocumentId = Guid.Parse("C0103C26-E7C9-4008-3F66-1B2DB192177E"),
                    CanonicalName = "not-in-sitemap",
                    BreadcrumbTitle = article,
                    IncludeInSitemap = false,
                    LastReviewed = DateTime.UtcNow,
                },
            };

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            helpPageModels.ForEach(f => client.PostAsync(url, f, new JsonMediaTypeFormatter()));
        }
    }
}
