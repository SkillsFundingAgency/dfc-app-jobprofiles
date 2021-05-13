using AutoMapper;
using DFC.App.JobProfile.ContentAPI.Configuration;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ContentAPI.Services
{
    [ExcludeFromCodeCoverage]
    internal sealed class GraphContentProvider :
        IProvideGraphContent,
        IRequireServiceRegistration
    {
        private static readonly string[] _relationshipStubs = new string[]
       {
        "HasSocCode",
        "HasRelatedCareer",
       };

        private readonly IContentApiConfiguration _clientConfig;
        private readonly IApiDataProcessorService _dataProcessor;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly IProcessGraphCuries _curieProcessor;
        private readonly IApiCacheService _cacheService;

        public GraphContentProvider(
            IContentApiConfiguration clientConfig,
            IApiDataProcessorService apiDataProcessor,
            HttpClient httpClient,
            IMapper mapper,
            IProcessGraphCuries curieProcessor,
            IApiCacheService cacheService)
        {
            _clientConfig = clientConfig;
            _dataProcessor = apiDataProcessor;
            _httpClient = httpClient;
            _mapper = mapper;
            _curieProcessor = curieProcessor;
            _cacheService = cacheService;
        }

        Task<IReadOnlyCollection<TApiModel>> IProvideGraphContent.GetSummaryItems<TApiModel>()
        {
            var summaryEndpoint = new Uri(
                  $"{_clientConfig.BaseAddress}{_clientConfig.SummaryEndpoint}",
                  UriKind.Absolute);

            return GetSummaryItemsFor<TApiModel>(summaryEndpoint);
        }

        public async Task<TRoot> GetContentItem<TRoot, TLinkedItem>(Uri uri)
            where TRoot : class, IRootContentItem<TLinkedItem>, new()
            where TLinkedItem : class, ILinkedContentItem<TLinkedItem>, new()
        {
            var apiDataModel = await _dataProcessor.GetAsync<TRoot>(_httpClient, uri) ?? new TRoot();
            var links = _curieProcessor.GetContentItemLinkedItems(apiDataModel);
            var candidates = await GetLinkedContentItems<TLinkedItem>(links);
            candidates.ForEach(apiDataModel.ContentItems.Add);

            return apiDataModel;
        }

        public async Task<TLinkedItem> GetLinkedItem<TLinkedItem>(Uri uri)
            where TLinkedItem : class, ILinkedContentItem<TLinkedItem>, new() =>
                await _dataProcessor.GetAsync<TLinkedItem>(_httpClient, uri) ?? new TLinkedItem();

        public async Task<IReadOnlyCollection<TApiModel>> GetStaticItems<TApiModel>()
            where TApiModel : class
        {
            var contentList = new List<TApiModel>();

            foreach (var id in _clientConfig.PageStaticContentIDs)
            {
                var url = new Uri(
                    $"{_clientConfig.BaseAddress}{_clientConfig.StaticContentEndpoint}{id}",
                    UriKind.Absolute);

                var content = await _dataProcessor.GetAsync<TApiModel>(_httpClient, url);

                if (content != null)
                {
                    contentList.Add(content);
                }
            }

            return contentList;
        }

        private async Task<IReadOnlyCollection<TApiModel>> GetSummaryItemsFor<TApiModel>(Uri thisResource)
             where TApiModel : class, IResourceLocatable
        {
            var result = await _dataProcessor
                .GetAsync<IReadOnlyCollection<TApiModel>>(_httpClient, thisResource)
                ?? new List<TApiModel>();

            _cacheService.Clear();

            return result;
        }

        private async Task<List<TLinkedItem>> GetLinkedContentItems<TLinkedItem>(IReadOnlyCollection<IGraphRelation> relations)
            where TLinkedItem : class, ILinkedContentItem<TLinkedItem>, new()
        {
            var list = new List<TLinkedItem>();

            if (relations != null)
            {
                foreach (var relation in relations)
                {
                    await GetRelation(list, relation);
                }
            }

            return list;
        }

        private async Task GetRelation<TLinkedItem>(
            List<TLinkedItem> list,
            IGraphRelation relation)
                where TLinkedItem : class, ILinkedContentItem<TLinkedItem>, new()
        {
            if (relation != null && relation.Items != null)
            {
                foreach (var graphItem in relation.Items)
                {
                    await GetGraphItem(list, relation, graphItem);
                }
            }
        }

        private async Task GetGraphItem<TLinkedItem>(
            List<TLinkedItem> list,
            IGraphRelation relation,
            IGraphItem graphItem)
                 where TLinkedItem : class, ILinkedContentItem<TLinkedItem>, new()
        {
            if (graphItem != null && graphItem.Uri != null)
            {
                var descendent = GetFromApiCache<TLinkedItem>(graphItem.Uri);
                if (descendent != null)
                {
                    return;
                }

                descendent = await GetLinkedItem<TLinkedItem>(graphItem.Uri);

                if (!descendent.IsFaultedState())
                {
                    AddToApiCache(descendent);

                    _mapper.Map(graphItem, descendent);

                    list.Add(descendent);

                    if (!_relationshipStubs.Any(x => x.ComparesWith(relation.Relationship)))
                    {
                        var modelLinks = _curieProcessor.GetContentItemLinkedItems(descendent);
                        var candidates = await GetLinkedContentItems<TLinkedItem>(modelLinks);
                        candidates.ForEach(descendent.ContentItems.Add);
                    }
                }
            }
        }

        private void AddToApiCache<TModel>(TModel model)
            where TModel : class, ILinkedContentItem<TModel>
        {
            if (model.Uri == UriExtra.Empty)
            {
                throw new ArgumentException($"model.Url is invalid");
            }

            _cacheService.AddOrUpdate(model.Uri, model);
        }

        private TModel GetFromApiCache<TModel>(Uri uri)
            where TModel : class, ILinkedContentItem<TModel> => _cacheService.Retrieve<TModel>(uri);
    }
}
