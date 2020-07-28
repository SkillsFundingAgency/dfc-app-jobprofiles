using DFC.App.JobProfile.Data.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Repository.CosmosDb
{
    [ExcludeFromCodeCoverage]
    public class CosmosRepository<T> : ICosmosRepository<T>
        where T : IDataModel
    {
        private readonly CosmosDbConnection cosmosDbConnection;
        private readonly IDocumentClient documentClient;
        private readonly IHostingEnvironment env;

        public CosmosRepository(CosmosDbConnection cosmosDbConnection, IDocumentClient documentClient, IHostingEnvironment env)
        {
            this.cosmosDbConnection = cosmosDbConnection;
            this.documentClient = documentClient;
            this.env = env;
        }

        private Uri DocumentCollectionUri => UriFactory.CreateDocumentCollectionUri(cosmosDbConnection.DatabaseId, cosmosDbConnection.CollectionId);

        public async Task InitialiseDevEnvironment()
        {
            if (env.IsDevelopment())
            {
                await CreateDatabaseIfNotExistsAsync().ConfigureAwait(false);
                await CreateCollectionIfNotExistsAsync().ConfigureAwait(false);
            }
        }

        public async Task<bool> PingAsync()
        {
            var query = documentClient.CreateDocumentQuery<T>(DocumentCollectionUri, new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
                                       .AsDocumentQuery();

            if (query == null)
            {
                return false;
            }

            var models = await query.ExecuteNextAsync<T>().ConfigureAwait(false);
            var firstModel = models.FirstOrDefault();

            return firstModel != null;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = documentClient.CreateDocumentQuery<T>(DocumentCollectionUri, new FeedOptions { EnableCrossPartitionQuery = true })
                                      .AsDocumentQuery();

            var models = new List<T>();

            while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<T>().ConfigureAwait(false);

                models.AddRange(result);
            }

            return models.Any() ? models : null;
        }

        public async Task<HttpStatusCode> UpsertAsync(T model)
        {
            await InitialiseDevEnvironment().ConfigureAwait(false);

            var ac = new AccessCondition { Condition = model.Etag, Type = AccessConditionType.IfMatch };
            var pk = new PartitionKey(model.PartitionKey);

            try
            {
                var result = await documentClient.UpsertDocumentAsync(DocumentCollectionUri, model, new RequestOptions { AccessCondition = ac, PartitionKey = pk }).ConfigureAwait(false);

                return result.StatusCode;
            }
            catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                return HttpStatusCode.PreconditionFailed;
            }
        }

        public async Task<HttpStatusCode> DeleteAsync(Guid documentId)
        {
            var documentUri = CreateDocumentUri(documentId);

            var model = await GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);

            if (model != null)
            {
                var ac = new AccessCondition { Condition = model.Etag, Type = AccessConditionType.IfMatch };
                var pk = new PartitionKey(model.PartitionKey);

                var result = await documentClient.DeleteDocumentAsync(documentUri, new RequestOptions { AccessCondition = ac, PartitionKey = pk }).ConfigureAwait(false);

                return result.StatusCode;
            }

            return HttpStatusCode.NotFound;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            var query = documentClient.CreateDocumentQuery<T>(DocumentCollectionUri, new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
                                      .Where(where)
                                      .AsDocumentQuery();

            if (query == null)
            {
                return default(T);
            }

            var models = await query.ExecuteNextAsync<T>().ConfigureAwait(false);

            if (models != null && models.Count > 0)
            {
                return models.FirstOrDefault();
            }

            return default(T);
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(cosmosDbConnection.DatabaseId)).ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDatabaseAsync(new Database { Id = cosmosDbConnection.DatabaseId }).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(cosmosDbConnection.DatabaseId, cosmosDbConnection.CollectionId)).ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var pkDef = new PartitionKeyDefinition
                    {
                        Paths = new Collection<string>() { cosmosDbConnection.PartitionKey },
                    };

                    await documentClient.CreateDocumentCollectionAsync(
                                UriFactory.CreateDatabaseUri(cosmosDbConnection.DatabaseId),
                                new DocumentCollection { Id = cosmosDbConnection.CollectionId, PartitionKey = pkDef },
                                new RequestOptions { OfferThroughput = 1000 }).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }

        private Uri CreateDocumentUri(Guid documentId)
        {
            return UriFactory.CreateDocumentUri(cosmosDbConnection.DatabaseId, cosmosDbConnection.CollectionId, documentId.ToString());
        }
    }
}