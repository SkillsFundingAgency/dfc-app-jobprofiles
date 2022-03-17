using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DFC.App.JobProfile.Data.Models;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

using static JobProfile.Migration.CosmosChecker.Helpers;

namespace JobProfile.Migration.CosmosChecker
{
    public class CosmosDbComparator
    {
        static RetryOptions _retryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };

        private static readonly IDocumentClient _documentClientPre = new DocumentClient(
            new Uri("https://localhost:8081"),
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            new ConnectionPolicy
            {
                RetryOptions = _retryOptions
            });

        private static readonly IDocumentClient _documentClientPost = new DocumentClient(
            new Uri("https://localhost:8081"),
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            new ConnectionPolicy
            {
                RetryOptions = _retryOptions
            });

        private Uri DocumentCollectionUri => UriFactory.CreateDocumentCollectionUri("dfc-app-jobprofiles", "jobProfiles");

        public async Task CompareCosmosDb()
        {
            Console.WriteLine("Starting comparison...");
            int total = 0, matching = 0;
            var existingProfiles = await GetAllAsync(_documentClientPre);

            var newProfiles = await GetAllAsync(_documentClientPost);
            foreach (var profile in existingProfiles)
            {
                total++;
                var newProfile = newProfiles.FirstOrDefault(p => p.CanonicalName.Equals(profile.CanonicalName));
                (bool doesMatch, string property) = DoesMatch(newProfile, profile);
                if (doesMatch)
                {
                    matching++;
                    Console.WriteLine($"{total}: SUCCESS: Cosmos document matched for profile: {profile.CanonicalName}");
                }
                else
                {
                    Console.WriteLine($"{total}: FAILED: Cosmos document does not match for profile: {profile.CanonicalName} - ({property}).");
                }

            }

            Console.WriteLine($"Finished comparison. Total profiles: {total}. Matching profiles: {matching}");
        }

        public (bool, string) DoesMatch(JobProfileModel newProfile, JobProfileModel oldProfile)
        {
            if (newProfile is null) return (false, "newProfile is null");
            if (oldProfile is null) return (false, "oldProfile is null");
            if (AreEqualOrNull(newProfile.JobProfileId, oldProfile.JobProfileId) is false) return (false, "JobProfileId");
            if (AreEqualOrNull(newProfile.BreadcrumbTitle, oldProfile.BreadcrumbTitle) is false) return (false, "BreadcrumbTitle");
            if (AreEqualOrNull(newProfile.MetaTags.Title, oldProfile.MetaTags.Title) is false) return (false, "MetaTags.Title");
            if (AreEqualOrNull(newProfile.MetaTags.Description, oldProfile.MetaTags.Description) is false) return (false, "MetaTags.Description");

            foreach (var segment in oldProfile.Segments)
            {
                var newSeg = newProfile.Segments.FirstOrDefault(s => s.Segment.Equals(segment.Segment));
                if (newSeg is null) return (false, segment.Segment.ToString());
                if (AreEqualOrNull(segment.Markup.Value, newSeg.Markup.Value) is false) return (false, $"{segment.Segment}.Markup");
                if (AreEqualOrNull(segment.JsonV1, newSeg.JsonV1) is false) return (false, $"{segment.Segment}.JsonV1");
            }

            return (true, string.Empty);
        }

        public async Task<IEnumerable<JobProfileModel>> GetAllAsync(IDocumentClient client)
        {
            var query = client.CreateDocumentQuery<JobProfileModel>(DocumentCollectionUri, new FeedOptions { EnableCrossPartitionQuery = true })
                                      .AsDocumentQuery();

            var models = new List<JobProfileModel>();

            while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<JobProfileModel>();
                models.AddRange(result);
            }

            return models;
        }
    }
}
