using AutoMapper;
using DFC.App.JobProfile.ContentAPI.Configuration;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ContentAPI.Services
{
    internal sealed class GraphContentProvider :
        IProvideGraphContent,
        IRequireServiceRegistration
    {
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

        public async Task<TRoot> GetComposedItem<TRoot, TBranch>(Uri uri)
            where TRoot : class, IRootContentItem<TBranch>, new()
            where TBranch : class, IBranchContentItem<TBranch>, new()
        {
            var apiDataModel = await _dataProcessor
                .GetAsync<TRoot>(_httpClient, uri)
                    ?? new TRoot();

            var links = _curieProcessor.GetRelations(apiDataModel);
            var candidates = await GetBranchedContentItems<TBranch>(links);
            candidates.ForEach(apiDataModel.ContentItems.Add);

            return apiDataModel;
        }

        public async Task<TBranch> GetBranchItem<TBranch>(Uri uri)
            where TBranch : class, IBranchContentItem<TBranch>, new() =>
                await _dataProcessor.GetAsync<TBranch>(_httpClient, uri)
                    ?? new TBranch();

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

        private async Task<List<TBranch>> GetBranchedContentItems<TBranch>(IReadOnlyCollection<IGraphRelation> relations)
            where TBranch : class, IBranchContentItem<TBranch>, new()
        {
            var list = new List<TBranch>();

            foreach (var relation in relations)
            {
                foreach (var link in relation.Items)
                {
                    if (link != null)
                    {
                        var descendent = GetFromApiCache<TBranch>(link.Uri);

                        if (descendent == null)
                        {
                            descendent = await GetBranchItem<TBranch>(link.Uri);

                            if (!descendent.IsFaultedState())
                            {
                                AddToApiCache(descendent);

                                _mapper.Map(link, descendent);

                                list.Add(descendent);

                                if (_clientConfig.RelationshipStubs.Any(x => x.ComparesWith(relation.Relationship)))
                                {
                                    continue;
                                }

                                var modelLinks = _curieProcessor.GetRelations(descendent);
                                var candidates = await GetBranchedContentItems<TBranch>(modelLinks);
                                candidates.ForEach(descendent.ContentItems.Add);
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
                    ?? new List<TApiModel>();

            _cacheService.Clear();

            return result;
        }

        private void AddToApiCache<TModel>(TModel model)
            where TModel : class, IBranchContentItem<TModel>
        {
            if (model.Uri == UriExtra.Empty)
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
