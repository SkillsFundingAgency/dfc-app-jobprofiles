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
            const string url = "/profile";
            var models = new List<CreateOrUpdateJobProfileModel>()
            {
                new CreateOrUpdateJobProfileModel()
                {
                    DocumentId = articleGuid,
                    CanonicalName = article,
                },
                new CreateOrUpdateJobProfileModel()
                {
                    DocumentId = Guid.Parse("C16B389D-91AD-4F3D-2485-9F7EE953AFE4"),
                    CanonicalName = $"{article}-2",
                },
                new CreateOrUpdateJobProfileModel()
                {
                    DocumentId = Guid.Parse("C0103C26-E7C9-4008-3F66-1B2DB192177E"),
                    CanonicalName = $"{article}-3",
                },
            };

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            models.ForEach(f => client.PostAsync(url, f, new JsonMediaTypeFormatter()));
        }
    }
}
