// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.EventProcessing.Models;
using DFC.App.JobProfile.EventProcessing.Services;
using DFC.App.Services.Common.Registration;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

//using IContentCacheService = DFC.Content.Pkg.Netcore.Data.Contracts.IContentCacheService;

namespace DFC.App.JobProfile.Webhooks.Services
{
    internal sealed class WebhooksProvider :
        IProvideWebhooks,
        IRequireServiceRegistration
    {
        private readonly ILogger<WebhooksProvider> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IEventMessageService<JobProfileCached> _eventMessage;
        private readonly IProvideGraphContent _graphContent;
        private readonly IContentPageService<JobProfileCached> _pageContent;
        //private readonly IContentCacheService _otherCacheThingy;
        private readonly IEventGridService<JobProfileCached> _eventGrid;

        public WebhooksProvider(
            ILogger<WebhooksProvider> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<JobProfileCached> eventMessageService,
            IProvideGraphContent cmsApiService,
            IContentPageService<JobProfileCached> pageService,
            //IContentCacheService otherCacheThingy,
            IEventGridService<JobProfileCached> eventGridService)
        {
            _logger = logger;
            _mapper = mapper;
            _eventMessage = eventMessageService;
            _graphContent = cmsApiService;
            _pageContent = pageService;
            //_otherCacheThingy = otherCacheThingy;
            _eventGrid = eventGridService;
        }

        public async Task<HttpStatusCode> ProcessMessage(
            EventOperation eventOperation,
            Guid eventId,
            Guid contentId,
            string apiEndpoint)
        {
            switch (eventOperation)
            {
                case EventOperation.Delete:
                    return await DeleteContentItem(contentId);

                case EventOperation.CreateOrUpdate:
                    if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri url))
                    {
                        throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    }

                    return await ProcessContentItem(url, contentId);

                default:
                    _logger.LogError($"Event Id: {eventId} got unknown cache operation - {eventOperation}");
                    return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> ProcessContentItem(Uri url, Guid contentItemId)
        {
            var apiDataModel = await _graphContent.GetComposedItem<ContentApiRootElement, ContentApiBranchElement>(url);
            var contentPageModel = _mapper.Map<JobProfileCached>(apiDataModel);

            if (contentPageModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            var existingContentPageModel = await _pageContent.GetByIdAsync(contentItemId);

            var contentResult = await _eventMessage.UpdateAsync(contentPageModel);

            if (contentResult == HttpStatusCode.NotFound)
            {
                contentResult = await _eventMessage.CreateAsync(contentPageModel);
            }

            if (contentResult == HttpStatusCode.OK
                || contentResult == HttpStatusCode.Created)
            {
                await _eventGrid.CompareThenSendEvent(existingContentPageModel, contentPageModel);

                // TODO: don't understand what this does...
                //var contentItemIds = contentPageModel.AllContentItemIds.ToList();
                //contentCacheService.AddOrReplace(contentItemId, contentItemIds);
            }

            return contentResult;
        }

        public async Task<HttpStatusCode> DeleteContentItem(Guid contentItemId)
        {
            var existingContentPageModel = await _pageContent.GetByIdAsync(contentItemId);
            var result = await _eventMessage.DeleteAsync(contentItemId);

            if (result == HttpStatusCode.OK && existingContentPageModel != null)
            {
                await _eventGrid.SendEvent(EventOperation.Delete, existingContentPageModel);

                // TODO: don't understand what this does...
                //_otherCacheThingy.Remove(contentItemId);
            }

            return result;
        }

        public ContentApiBranchElement FindContentItem(Guid contentItemId, ICollection<ContentApiBranchElement> items)
        {
            if (items == null || !items.Any())
            {
                return default;
            }

            foreach (var contentItemModel in items)
            {
                if (contentItemModel.Id == contentItemId)
                {
                    return contentItemModel;
                }

                var childContentItemModel = FindContentItem(contentItemId, contentItemModel.ContentItems);

                if (childContentItemModel != null)
                {
                    return childContentItemModel;
                }
            }

            return default;
        }

        public bool RemoveContentItem(Guid contentItemId, ICollection<ContentApiBranchElement> items)
        {
            if (items == null || !items.Any())
            {
                return false;
            }

            foreach (var contentItemModel in items)
            {
                if (contentItemModel.Id == contentItemId)
                {
                    items.Remove(contentItemModel);
                    return true;
                }

                var removedContentitem = RemoveContentItem(contentItemId, contentItemModel.ContentItems);

                if (removedContentitem)
                {
                    return removedContentitem;
                }
            }

            return false;
        }

        public bool TryValidate(JobProfileCached contentModel)
        {
            _ = contentModel ?? throw new ArgumentNullException(nameof(contentModel));

            var validationContext = new ValidationContext(contentModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(contentModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    _logger.LogError($"Error validating {contentModel.CanonicalName} - {contentModel.Uri}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }
    }
}
