using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DFC.App.JobProfile.Data.Models;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace JobProfile.Migration.CosmosChecker
{
    public static class Program
    {
        private static readonly CosmosDbComparator cosmosComparator = new CosmosDbComparator();
        private static readonly AzureSearchComparator azureIndexComparator = new AzureSearchComparator();

        private static readonly string databaseName = "dfc-cms-data";
        private static readonly string collectionName = "preview";

        static async Task Main(string[] args)
        {
            //await DeleteProfilesFromDataSync();
            await cosmosComparator.CompareCosmosDb();
            //await azureIndexComparator.CompareAzureCache();
        }

        private async static Task DeleteProfilesFromDataSync()
        {
            IDocumentClient documentClient = new DocumentClient(
               new Uri("https://localhost:443"),
               "asd==",
               new ConnectionPolicy
               {
                   RetryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 }
               });

            var newProfiles = await GetAllAsync(documentClient);
            foreach (var profile in newProfiles)
            {
                await documentClient.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(databaseName, collectionName, profile.DocumentId.ToString()),
                new RequestOptions { PartitionKey = new PartitionKey("jobprofile") });
            }
        }

        public async static Task<IEnumerable<JobProfileModel>> GetAllAsync(IDocumentClient client)
        {
            var query = client.CreateDocumentQuery<JobProfileModel>(
                UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                "SELECT * FROM c WHERE c.ContentType = 'jobprofile'",
                new FeedOptions { EnableCrossPartitionQuery = true })
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
