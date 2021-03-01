using DFC.App.Services.Common.Registration;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.EventProcessing.Services
{
    public class EventMessageService<TModel> :
        IEventMessageService<TModel>,
        IRequireServiceRegistration
        where TModel : class, IContentPageModel
    {
        private readonly ILogger<EventMessageService<TModel>> _logger;
        private readonly IContentPageService<TModel> _pageService;

        public EventMessageService(
            ILogger<EventMessageService<TModel>> logger,
            IContentPageService<TModel> contentPageService)
        {
            _logger = logger;
            _pageService = contentPageService;
        }

        public async Task<IList<TModel>> GetAllCachedCanonicalNamesAsync()
        {
            var serviceDataModels = await _pageService.GetAllAsync().ConfigureAwait(false);

            return serviceDataModels?.ToList();
        }

        public async Task<HttpStatusCode> CreateAsync(TModel upsertDocumentModel)
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await _pageService.GetByIdAsync(upsertDocumentModel.Id).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var response = await _pageService.UpsertAsync(upsertDocumentModel).ConfigureAwait(false);

            _logger.LogInformation($"{nameof(CreateAsync)} has upserted content for: {upsertDocumentModel.CanonicalName} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> UpdateAsync(TModel upsertDocumentModel)
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await _pageService.GetByIdAsync(upsertDocumentModel.Id).ConfigureAwait(false);
            if (existingDocument == null)
            {
                return HttpStatusCode.NotFound;
            }

            upsertDocumentModel.Etag = existingDocument.Etag;

            var response = await _pageService.UpsertAsync(upsertDocumentModel).ConfigureAwait(false);

            _logger.LogInformation($"{nameof(UpdateAsync)} has upserted content for: {upsertDocumentModel.CanonicalName} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> DeleteAsync(Guid id)
        {
            var isDeleted = await _pageService.DeleteAsync(id).ConfigureAwait(false);

            if (isDeleted)
            {
                _logger.LogInformation($"{nameof(DeleteAsync)} has deleted content for document Id: {id}");
                return HttpStatusCode.OK;
            }
            else
            {
                _logger.LogWarning($"{nameof(DeleteAsync)} has returned no content for: {id}");
                return HttpStatusCode.NotFound;
            }
        }
    }
}
