// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
//using DFC.Content.Pkg.Netcore.Data.Contracts;
//using DFC.Content.Pkg.Netcore.Data.Enums;
//using DFC.Content.Pkg.Netcore.Data.Models;
//using System;
//using System.Collections.Generic;

//namespace DFC.App.JobProfile.Cacheing
//{
//    public class ContentCacheService :
//        IContentCacheService
//    {
//        private IDictionary<Guid, List<Guid>> ContentItems { get; set; } = new Dictionary<Guid, List<Guid>>();

//        public ContentCacheStatus CheckIsContentItem(Guid contentItemId)
//        {
//            foreach (var contentId in ContentItems.Keys)
//            {
//                if (ContentItems[contentId].Contains(contentItemId))
//                {
//                    return ContentCacheStatus.ContentItem;
//                }
//            }

//            return ContentCacheStatus.NotFound;
//        }

//        public void Clear()
//        {
//            ContentItems.Clear();
//        }

//        public IList<Guid> GetContentIdsContainingContentItemId(Guid contentItemId)
//        {
//            var contentIds = new List<Guid>();

//            foreach (var contentId in ContentItems.Keys)
//            {
//                if (ContentItems[contentId].Contains(contentItemId))
//                {
//                    contentIds.Add(contentId);
//                }
//            }

//            return contentIds;
//        }

//        public void Remove(Guid contentId)
//        {
//            if (ContentItems.ContainsKey(contentId))
//            {
//                ContentItems.Remove(contentId);
//            }
//        }

//        public void RemoveContentItem(Guid contentId, Guid contentItemId)
//        {
//            if (ContentItems.ContainsKey(contentId))
//            {
//                ContentItems[contentId].Remove(contentItemId);
//            }
//        }

//        public void AddOrReplace(Guid contentId, List<Guid> contentItemIds, string parentContentType = "default")
//        {
//            if (ContentItems.ContainsKey(contentId))
//            {
//                ContentItems[contentId] = contentItemIds;
//            }
//            else
//            {
//                ContentItems.Add(contentId, contentItemIds);
//            }
//        }

//        public IEnumerable<ContentCacheResult> GetContentCacheStatus(Guid contentItemId)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
