using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Services
{
    public class StaticContentReloadService : IStaticContentReloadService
    {
        private readonly ILogger<StaticContentReloadService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IDocumentService<StaticContentItemModel> staticContentDocumentService;
        private readonly ICmsApiService cmsApiService;
        private readonly CmsApiClientOptions cmsApiClientOptions;

        public StaticContentReloadService(
            ILogger<StaticContentReloadService> logger,
            AutoMapper.IMapper mapper,
            IDocumentService<StaticContentItemModel> staticContentDocumentService,
            ICmsApiService cmsApiService,
            CmsApiClientOptions cmsApiClientOptions)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.staticContentDocumentService = staticContentDocumentService;
            this.cmsApiService = cmsApiService;
            this.cmsApiClientOptions = cmsApiClientOptions;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Reload All shared-content cache started");

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload shared-content cache cancelled");

                    return;
                }

                await ReloadSharedContent(stoppingToken).ConfigureAwait(false);

                logger.LogInformation("Reload All shared-content cache completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in shared-content cache reload");
                throw;
            }
        }

        public async Task ReloadSharedContent(CancellationToken stoppingToken)
        {
            await staticContentDocumentService.PurgeAsync().ConfigureAwait(false);

            var contentIds = cmsApiClientOptions.ContentIds.Split(",", StringSplitOptions.RemoveEmptyEntries);

            foreach (var contentId in contentIds)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload shared-content cache cancelled");

                    return;
                }

                var apiDataModel = await cmsApiService.GetItemAsync<StaticContentItemApiDataModel>("sharedcontent", new Guid(contentId)).ConfigureAwait(false);

                if (apiDataModel == null)
                {
                    logger.LogError($"Shared-content: {contentId} not found in API response");
                }

                var staticContentItemModel = mapper.Map<StaticContentItemModel>(apiDataModel);

                if (!TryValidateModel(staticContentItemModel))
                {
                    logger.LogError($"Validation failure for {staticContentItemModel.Title} - {staticContentItemModel.Url}");

                    return;
                }

                await staticContentDocumentService.UpsertAsync(staticContentItemModel).ConfigureAwait(false);
            }
        }

        public bool TryValidateModel<TModel>(TModel model)
            where TModel : class, ICachedModel
        {
            _ = model ?? throw new ArgumentNullException(nameof(model));

            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {model.Title} - {model.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }
    }
}
