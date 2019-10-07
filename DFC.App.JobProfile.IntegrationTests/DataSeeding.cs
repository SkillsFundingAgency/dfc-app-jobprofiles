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

        public static Guid DefaultArticleGuid => Guid.Parse("63DEA97E-B61C-4C14-15DC-1BD08EA20DC8");

        public static void SeedDefaultArticle(CustomWebApplicationFactory<Startup> factory)
        {
            const string url = "/profile";
            var models = new List<JobProfileModel>()
            {
                new JobProfileModel()
                {
                    DocumentId = DefaultArticleGuid,
                    CanonicalName = DefaultArticleName,
                    SocLevelTwo = "12",
                    LastReviewed = DateTime.UtcNow,
                },
                new JobProfileModel()
                {
                    DocumentId = Guid.Parse("C16B389D-91AD-4F3D-2485-9F7EE953AFE4"),
                    CanonicalName = $"{DefaultArticleName}-2",
                    SocLevelTwo = "34",
                    LastReviewed = DateTime.UtcNow,
                },
                new JobProfileModel()
                {
                    DocumentId = Guid.Parse("C0103C26-E7C9-4008-3F66-1B2DB192177E"),
                    CanonicalName = $"{DefaultArticleName}-3",
                    SocLevelTwo = "56",
                    LastReviewed = DateTime.UtcNow,
                },
            };

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            models.ForEach(f => client.PostAsync(url, f, new JsonMediaTypeFormatter()).GetAwaiter().GetResult());
        }
    }
}
