using DFC.App.JobProfile.Data.Models;
using System;
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
            var models = new List<Data.Models.JobProfileModel>()
            {
                new Data.Models.JobProfileModel()
                {
                    CanonicalName = DefaultArticleName,
                    SocLevelTwo = "12",
                    LastReviewed = DateTime.UtcNow,
                    IncludeInSitemap = true,
                    MetaTags = new MetaTags
                    {
                        Title = $"This is a title",
                    },
                    SequenceNumber = 1,
                },
                new Data.Models.JobProfileModel()
                {
                    CanonicalName = $"{DefaultArticleName}-2",
                    SocLevelTwo = "34",
                    LastReviewed = DateTime.UtcNow,
                    IncludeInSitemap = true,
                    MetaTags = new MetaTags
                    {
                        Title = $"This is a title",
                    },
                    SequenceNumber = 2,
                },
                new Data.Models.JobProfileModel()
                {
                    CanonicalName = $"{DefaultArticleName}-3",
                    SocLevelTwo = "56",
                    LastReviewed = DateTime.UtcNow,
                    IncludeInSitemap = true,
                    MetaTags = new MetaTags
                    {
                        Title = $"This is a title",
                    },
                    SequenceNumber = 3,
                },
            };

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            models.ForEach(f => client.PostAsync(url, f, new JsonMediaTypeFormatter()).GetAwaiter().GetResult());
        }
    }
}
