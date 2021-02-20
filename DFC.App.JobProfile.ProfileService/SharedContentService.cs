// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
//using AutoMapper;
//using DFC.App.JobProfile.Data.Contracts;
//using DFC.App.JobProfile.Data.Models;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace DFC.App.JobProfile.ProfileService
//{
//    public class SharedContentService : ISharedContentService
//    {
//        private readonly IStaticCosmosRepository<StaticContentItemModel> repository;

//        // TODO: remove me!
//        // private readonly IMapper mapper
//        public SharedContentService(
//            IStaticCosmosRepository<StaticContentItemModel> repository,
//            IMapper mapper)
//        {
//            this.repository = repository;
//        }

//        public async Task<StaticContentItemModel> GetByNameAsync(string canonicalName)
//        {
//            if (string.IsNullOrWhiteSpace(canonicalName))
//            {
//                throw new ArgumentNullException(nameof(canonicalName));
//            }

//            return await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
//        }

//        public async Task<List<StaticContentItemModel>> GetByNamesAsync(List<string> contentList)
//        {
//            var contentListItems = new List<StaticContentItemModel>();

//            if (contentList == null)
//            {
//                throw new ArgumentNullException(nameof(contentList));
//            }

//            foreach (var item in contentList)
//            {
//                var sharedContentItem = await repository.GetAsync(d => d.CanonicalName == item.ToLowerInvariant()).ConfigureAwait(false);

//                if (sharedContentItem != null)
//                {
//                    contentListItems.Add(sharedContentItem);
//                }
//            }

//            return contentListItems;
//        }
//    }
//}
