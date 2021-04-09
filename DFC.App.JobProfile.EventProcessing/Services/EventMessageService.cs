using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.EventProcessing.Services
{
    [ExcludeFromCodeCoverage]
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
            var serviceDataModels = await _pageService.GetAllAsync();

            return serviceDataModels?.ToList();
        }

        public async Task<HttpStatusCode> CreateAsync(TModel upsertDocumentModel)
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await _pageService.GetByIdAsync(upsertDocumentModel.Id);
            if (existingDocument != null)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var response = await _pageService.UpsertAsync(upsertDocumentModel);

            _logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} has upserted content for: {upsertDocumentModel.CanonicalName} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> UpdateAsync(TModel upsertDocumentModel)
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await _pageService.GetByIdAsync(upsertDocumentModel.Id);
            if (existingDocument == null)
            {
                return HttpStatusCode.NotFound;
            }

            upsertDocumentModel.Etag = existingDocument.Etag;

            var response = await _pageService.UpsertAsync(upsertDocumentModel);

            _logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} has upserted content for: {upsertDocumentModel.CanonicalName} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> DeleteAsync(Guid id)
        {
            var isDeleted = await _pageService.DeleteAsync(id);

            if (isDeleted)
            {
                _logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} has deleted content for document Id: {id}");
                return HttpStatusCode.OK;
            }
            else
            {
                _logger.LogWarning($"{Utils.LoggerMethodNamePrefix()} has returned no content for: {id}");
                return HttpStatusCode.NotFound;
            }
        }
    }
}
