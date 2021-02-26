using DFC.Compui.Cosmos.Contracts;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.DocumentStore
{
    public class DocumentStoreManager<TModel> :
        IManageDocumentStorage<TModel>
            where TModel : IContentPageModel
    {
        private readonly JobProfileStorageProperties _connectionProperties;
        private readonly IDocumentClient _documentClient;
        private readonly bool _isDevelopement;

        public DocumentStoreManager(JobProfileStorageProperties cosmosDbConnection, IDocumentClient documentClient, bool isDevelopement = false)
        {
            _connectionProperties = cosmosDbConnection;
            _documentClient = documentClient;
            _isDevelopement = isDevelopement;
        }

        private Uri DocumentCollectionUri => UriFactory.CreateDocumentCollectionUri(_connectionProperties.DatabaseName, _connectionProperties.CollectionName);

        public async Task InitialiseDevEnvironment()
        {
            if (_isDevelopement)
            {
                await CreateDatabaseIfNotExists().ConfigureAwait(false);
                await CreateCollectionIfNotExists().ConfigureAwait(false);
            }
        }

        public async Task<bool> Ping()
        {
            var query = _documentClient
                .CreateDocumentQuery<TModel>(DocumentCollectionUri, new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
                .AsDocumentQuery();

            if (query == null)
            {
                return false;
            }

            var models = await query.ExecuteNextAsync<TModel>().ConfigureAwait(false);
            var firstModel = models.FirstOrDefault();

            return firstModel != null;
        }

        public async Task<IReadOnlyCollection<TModel>> GetAll()
        {
            var query = _documentClient
                .CreateDocumentQuery<TModel>(DocumentCollectionUri, new FeedOptions { EnableCrossPartitionQuery = true })
                .AsDocumentQuery();

            var models = new List<TModel>();

            while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<TModel>().ConfigureAwait(false);

                models.AddRange(result);
            }

            return models;
        }

        public async Task<HttpStatusCode> Upsert(TModel model)
        {
            await InitialiseDevEnvironment().ConfigureAwait(false);

            var ac = new AccessCondition { Condition = model.Etag, Type = AccessConditionType.IfMatch };
            var pk = new PartitionKey(model.PartitionKey);

            try
            {
                var result = await _documentClient.UpsertDocumentAsync(DocumentCollectionUri, model, new RequestOptions { AccessCondition = ac, PartitionKey = pk }).ConfigureAwait(false);

                return result.StatusCode;
            }
            catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                return HttpStatusCode.PreconditionFailed;
            }
        }

        public async Task<HttpStatusCode> Delete(Guid documentID)
        {
            var documentUri = CreateDocumentUri(documentID);

            var model = await Get(d => d.Id == documentID).ConfigureAwait(false);

            if (model != null)
            {
                var ac = new AccessCondition { Condition = model.Etag, Type = AccessConditionType.IfMatch };
                var pk = new PartitionKey(model.PartitionKey);

                var result = await _documentClient.DeleteDocumentAsync(documentUri, new RequestOptions { AccessCondition = ac, PartitionKey = pk }).ConfigureAwait(false);

                return result.StatusCode;
            }

            return HttpStatusCode.NotFound;
        }

        public async Task<TModel> Get(Expression<Func<TModel, bool>> where)
        {
            var query = _documentClient
                .CreateDocumentQuery<TModel>(DocumentCollectionUri, new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
                .Where(where)
                .AsDocumentQuery();

            if (query == null)
            {
                return default(TModel);
            }

            var models = await query.ExecuteNextAsync<TModel>().ConfigureAwait(false);

            if (models != null && models.Count > 0)
            {
                return models.FirstOrDefault();
            }

            return default(TModel);
        }

        private async Task CreateDatabaseIfNotExists()
        {
            try
            {
                await _documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_connectionProperties.DatabaseName)).ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _documentClient.CreateDatabaseAsync(new Database { Id = _connectionProperties.DatabaseName }).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExists()
        {
            try
            {
                await _documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_connectionProperties.DatabaseName, _connectionProperties.CollectionName)).ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var pkDef = new PartitionKeyDefinition
                    {
                        Paths = new Collection<string>() { _connectionProperties.PartitionKey },
                    };

                    await _documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_connectionProperties.DatabaseName),
                        new DocumentCollection { Id = _connectionProperties.CollectionName, PartitionKey = pkDef },
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
            return UriFactory.CreateDocumentUri(_connectionProperties.DatabaseName, _connectionProperties.CollectionName, documentId.ToString());
        }
    }
}