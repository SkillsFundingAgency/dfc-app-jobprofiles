// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.App.JobProfile.EventProcessing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Cacheing
{
    public class StaticContentLoader :
        ILoadStaticContent
    {
        private readonly ILogger<StaticContentLoader> _logger;
        private readonly IEventMessageService<ContentApiStaticElement> _messageService;
        private readonly IProvideGraphContent _graphContent;
        //private readonly IContentCacheService _otherCacheThingy;

        public StaticContentLoader(
            ILogger<StaticContentLoader> logger,
            IEventMessageService<ContentApiStaticElement> eventMessageService,
            IProvideGraphContent cmsApiService)
            //,IContentCacheService contentCacheService)
        {
            _logger = logger;
            _messageService = eventMessageService;
            _graphContent = cmsApiService;
            //_otherCacheThingy = contentCacheService;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Reload static content started");

                var staticContent = await _graphContent.GetStaticItems<ContentApiStaticElement>().ConfigureAwait(false);

                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Reload static content cancelled");

                    return;
                }

                if (staticContent != null)
                {
                    await ProcessContentAsync(staticContent, stoppingToken).ConfigureAwait(false);

                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("Reload static content cancelled");

                        return;
                    }
                }

                _logger.LogInformation("Reload static content completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in static content reload");
            }
        }

        public async Task ProcessContentAsync(
            IReadOnlyCollection<ContentApiStaticElement> sharedContent,
            CancellationToken stoppingToken)
        {
            _logger.LogInformation("Process summary list started");

            //_otherCacheThingy.Clear();

            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Process summary list cancelled");

                return;
            }

            await GetAndSaveItemAsync(sharedContent, stoppingToken).ConfigureAwait(false);

            _logger.LogInformation("Process summary list completed");
        }

        public async Task GetAndSaveItemAsync(
            IReadOnlyCollection<ContentApiStaticElement> items,
            CancellationToken stoppingToken)
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));
            await Task.CompletedTask;

            foreach (var item in items)
            {
                item.PartitionKey = "/";
                item.CanonicalName = item.Title.Replace(" ", "-").ToLower();

                try
                {
                    _logger.LogInformation($"Updating static content cache with {item.Id} - {item.Url}");

                    var result = await _messageService.UpdateAsync(item).ConfigureAwait(false);

                    if (result == HttpStatusCode.NotFound)
                    {
                        _logger.LogInformation($"Does not exist, creating static content cache with {item.Id} - {item.Url}");

                        result = await _messageService.CreateAsync(item).ConfigureAwait(false);

                        if (result == HttpStatusCode.OK)
                        {
                            _logger.LogInformation($"Created static content cache with {item.Id} - {item.Url}");
                        }
                        else
                        {
                            _logger.LogError($"Static content cache create error status {result} from {item.Id} - {item.Url}");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Updated static content cache with {item.Id} - {item.Url}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in get and save for {item.Id} - {item.Url}");
                }
            }
        }
    }
}
