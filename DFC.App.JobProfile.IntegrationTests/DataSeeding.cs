using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace DFC.App.JobProfile.IntegrationTests
{
    public static class DataSeeding
    {
        public const string DefaultArticleName = "profile-article";

        public static void SeedDefaultArticle(CustomWebApplicationFactory<Startup> factory)
        {
            const string url = "/profile";
            var models = new List<JobProfileModel>()
            {
                new JobProfileModel()
                {
                    CanonicalName = DefaultArticleName,
                    IncludeInSitemap = true,
                    MetaTags = new MetaTags
                    {
                        Title = $"This is a title",
                    },
                },
                new JobProfileModel()
                {
                    CanonicalName = $"{DefaultArticleName}-2",
                    IncludeInSitemap = true,
                    MetaTags = new MetaTags
                    {
                        Title = $"This is a title",
                    },
                },
                new JobProfileModel()
                {
                    CanonicalName = $"{DefaultArticleName}-3",
                    IncludeInSitemap = true,
                    MetaTags = new MetaTags
                    {
                        Title = $"This is a title",
                    },
                },
            };

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            models.ForEach(f => client.PostAsync(url, f, new JsonMediaTypeFormatter()).GetAwaiter().GetResult());
        }
    }
}
