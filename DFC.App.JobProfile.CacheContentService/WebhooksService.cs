using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IContentCacheService = DFC.Content.Pkg.Netcore.Data.Contracts.IContentCacheService;

namespace DFC.App.JobProfile.CacheContentService
{
    public class WebhooksService :
        IWebhooksService
    {
        private readonly ILogger<WebhooksService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService<JobProfileCached> eventMessageService;
        private readonly IProvideGraphContent cmsApiService;
        private readonly IContentPageService<JobProfileCached> contentPageService;
        private readonly IContentCacheService contentCacheService;
        private readonly IEventGridService eventGridService;

        public WebhooksService(
            ILogger<WebhooksService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<JobProfileCached> eventMessageService,
            IProvideGraphContent cmsApiService,
            IContentPageService<JobProfileCached> contentPageService,
            IContentCacheService contentCacheService,
            IEventGridService eventGridService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.cmsApiService = cmsApiService;
            this.contentPageService = contentPageService;
            this.contentCacheService = contentCacheService;
            this.eventGridService = eventGridService;
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint)
        {
            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                        return await DeleteContentItemAsync(contentId).ConfigureAwait(false);

                case WebhookCacheOperation.CreateOrUpdate:

                    if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri url))
                    {
                        throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    }

                    return await ProcessContentItemAsync(url, contentId).ConfigureAwait(false);

                default:
                    logger.LogError($"Event Id: {eventId} got unknown cache operation - {webhookCacheOperation}");
                    return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> ProcessContentItemAsync(Uri url, Guid contentItemId)
        {
            var apiDataModel = await cmsApiService.GetComposedItem<ContentApiRootElement, ContentApiBranchElement>(url).ConfigureAwait(false);
            var contentPageModel = mapper.Map<JobProfileCached>(apiDataModel);

            if (contentPageModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            var existingContentPageModel = await contentPageService.GetByIdAsync(contentItemId).ConfigureAwait(false);

            var contentResult = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

            if (contentResult == HttpStatusCode.NotFound)
            {
                contentResult = await eventMessageService.CreateAsync(contentPageModel).ConfigureAwait(false);
            }

            if (contentResult == HttpStatusCode.OK || contentResult == HttpStatusCode.Created)
            {
                await eventGridService.CompareAndSendEventAsync(existingContentPageModel, contentPageModel).ConfigureAwait(false);

                var contentItemIds = contentPageModel.AllContentItemIds.ToList();

                contentCacheService.AddOrReplace(contentItemId, contentItemIds);
            }

            return contentResult;
        }

        public async Task<HttpStatusCode> DeleteContentItemAsync(Guid contentItemId)
        {
            var existingContentPageModel = await contentPageService.GetByIdAsync(contentItemId).ConfigureAwait(false);
            var result = await eventMessageService.DeleteAsync(contentItemId).ConfigureAwait(false);

            if (result == HttpStatusCode.OK && existingContentPageModel != null)
            {
                await eventGridService.SendEventAsync(WebhookCacheOperation.Delete, existingContentPageModel).ConfigureAwait(false);

                contentCacheService.Remove(contentItemId);
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
                if (contentItemModel.ItemID == contentItemId)
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
                if (contentItemModel.ItemID == contentItemId)
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

        public bool TryValidateModel(JobProfileCached contentPageModel)
        {
            _ = contentPageModel ?? throw new ArgumentNullException(nameof(contentPageModel));

            var validationContext = new ValidationContext(contentPageModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(contentPageModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {contentPageModel.CanonicalName} - {contentPageModel.Uri}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }
    }
}
