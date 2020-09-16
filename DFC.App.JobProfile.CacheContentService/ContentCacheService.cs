using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.enums;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.CacheContentService
{
    public class ContentCacheService : IContentCacheService
    {
        private IDictionary<Guid, List<Guid>> ContentItems { get; set; } = new Dictionary<Guid, List<Guid>>();

        public ContentCacheStatus CheckIsContentItem(Guid contentItemId)
        {
            foreach (var contentId in ContentItems.Keys)
            {
                if (ContentItems[contentId].Contains(contentItemId))
                {
                    return ContentCacheStatus.ContentItem;
                }
            }

            return ContentCacheStatus.NotFound;
        }

        public void Clear()
        {
            ContentItems.Clear();
        }

        public IList<Guid> GetContentIdsContainingContentItemId(Guid contentItemId)
        {
            var contentIds = new List<Guid>();

            foreach (var contentId in ContentItems.Keys)
            {
                if (ContentItems[contentId].Contains(contentItemId))
                {
                    contentIds.Add(contentId);
                }
            }

            return contentIds;
        }

        public void Remove(Guid contentId)
        {
            if (ContentItems.ContainsKey(contentId))
            {
                ContentItems.Remove(contentId);
            }
        }

        public void RemoveContentItem(Guid contentId, Guid contentItemId)
        {
            if (ContentItems.ContainsKey(contentId))
            {
                ContentItems[contentId].Remove(contentItemId);
            }
        }

        public void AddOrReplace(Guid contentId, List<Guid> contentItemIds)
        {
            if (ContentItems.ContainsKey(contentId))
            {
                ContentItems[contentId] = contentItemIds;
            }
            else
            {
                ContentItems.Add(contentId, contentItemIds);
            }
        }
    }
}
