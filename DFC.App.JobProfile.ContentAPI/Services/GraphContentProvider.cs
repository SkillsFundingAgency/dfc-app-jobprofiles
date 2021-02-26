using AutoMapper;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ContentAPI.Services
{
    public sealed class GraphContentProvider :
        IProvideGraphContent
    {
        private readonly ContentApiOptions _clientOptions;
        private readonly IApiDataProcessorService _dataProcessor;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly IProcessGraphCuries _curieProcessor;
        private readonly IApiCacheService _cacheService;

        public GraphContentProvider(
            ContentApiOptions clientOptions,
            IApiDataProcessorService apiDataProcessor,
            HttpClient httpClient,
            IMapper mapper,
            IProcessGraphCuries curieProcessor,
            IApiCacheService cacheService)
        {
            _clientOptions = clientOptions;
            _dataProcessor = apiDataProcessor;
            _httpClient = httpClient;
            _mapper = mapper;
            _curieProcessor = curieProcessor;
            _cacheService = cacheService;
        }

        public async Task<IReadOnlyCollection<TApiModel>> GetSummaryItems<TApiModel>()
            where TApiModel : class, IResourceLocatable
        {
            var summaryEndpoint = new Uri(
                  $"{_clientOptions.BaseAddress}{_clientOptions.SummaryEndpoint}",
                  UriKind.Absolute);

            return await GetSummaryItemsFor<TApiModel>(summaryEndpoint).ConfigureAwait(false);
        }

        public async Task<TRoot> GetComposedItem<TRoot, TBranch>(Uri uri)
            where TRoot : class, IRootContentItem<TBranch>, new()
            where TBranch : class, IBranchContentItem<TBranch>, new()
        {
            var apiDataModel = await _dataProcessor
                .GetAsync<TRoot>(_httpClient, uri)
                .ConfigureAwait(false)
                    ?? new TRoot();

            var links = _curieProcessor.GetRelations(apiDataModel);
            var candidates = await GetBranchedContentItems<TBranch>(links).ConfigureAwait(false);
            candidates.ForEach(apiDataModel.ContentItems.Add);

            return apiDataModel;
        }

        public async Task<TBranch> GetBranchItem<TBranch>(Uri uri)
            where TBranch : class, IBranchContentItem<TBranch>, new() =>
                await _dataProcessor.GetAsync<TBranch>(_httpClient, uri).ConfigureAwait(false)
                    ?? new TBranch();

        public async Task<IReadOnlyCollection<TApiModel>> GetStaticItems<TApiModel>()
            where TApiModel : class
        {
            var contentList = new List<TApiModel>();

            foreach (var id in _clientOptions.PageStaticContentIDs)
            {
                var url = new Uri(
                    $"{_clientOptions.BaseAddress}{_clientOptions.StaticContentEndpoint}{id}",
                    UriKind.Absolute);

                var content = await _dataProcessor.GetAsync<TApiModel>(_httpClient, url).ConfigureAwait(false);

                if (content != null)
                {
                    contentList.Add(content);
                }
            }

            return contentList;
        }

        private async Task<List<TBranch>> GetBranchedContentItems<TBranch>(IReadOnlyCollection<IGraphRelation> relations)
            where TBranch : class, IBranchContentItem<TBranch>, new()
        {
            var list = new List<TBranch>();

            foreach(var relation in relations)
            {
                foreach (var link in relation.Items)
                {
                    if (link != null)
                    {
                        var descendent = GetFromApiCache<TBranch>(link.Uri);

                        if (descendent == null)
                        {
                            descendent = await GetBranchItem<TBranch>(link.Uri).ConfigureAwait(false);

                            if (!descendent.IsFaultedState())
                            {
                                AddToApiCache(descendent);

                                _mapper.Map(link, descendent);

                                var modelLinks = _curieProcessor.GetRelations(descendent);
                                var candidates = await GetBranchedContentItems<TBranch>(modelLinks).ConfigureAwait(false);

                                candidates.ForEach(descendent.ContentItems.Add);

                                list.Add(descendent);
                            }
                        }
                    }
                }
            }

            return list;
        }

        private async Task<IReadOnlyCollection<TApiModel>> GetSummaryItemsFor<TApiModel>(Uri thisResource)
            where TApiModel : class, IResourceLocatable
        {
            var result = await _dataProcessor
                .GetAsync<IReadOnlyCollection<TApiModel>>(_httpClient, thisResource)
                .ConfigureAwait(false)
                    ?? new List<TApiModel>();

            _cacheService.Clear();

            return result;
        }

        private void AddToApiCache<TModel>(TModel model)
            where TModel : class, IBranchContentItem<TModel>
        {
            if (model.Uri == GraphSummaryItem.Empty)
            {
                throw new ArgumentException($"model.Url is invalid");
            }

            _cacheService.AddOrUpdate(model.Uri, model);
        }

        private TModel GetFromApiCache<TModel>(Uri uri)
            where TModel : class, IBranchContentItem<TModel> =>
                _cacheService.Retrieve<TModel>(uri);
    }
}
