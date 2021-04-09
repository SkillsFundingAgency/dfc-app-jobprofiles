using DFC.App.JobProfile.ContentAPI.Configuration;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.App.Services.Common.Registration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.JobProfile.ContentAPI.Services
{
    [ExcludeFromCodeCoverage]
    internal sealed class GraphCurieProcessor :
        IProcessGraphCuries,
        IRequireServiceRegistration
    {
        private readonly IContentApiConfiguration _options;

        public GraphCurieProcessor(IContentApiConfiguration options)
        {
            _options = options;
        }

        public IReadOnlyCollection<IGraphRelation> GetContentItemLinkedItems(IContainGraphLink container) =>
            GetLinkedItem(container.ContentLinks);

        internal IReadOnlyCollection<IGraphRelation> GetLinkedItem(JObject links)
        {
            var contentLinks = new List<IGraphRelation>();
            var baseReference = GetBaseReference(links);

            if (baseReference == null)
            {
                return contentLinks;
            }

            foreach (var (key, token) in links)
            {
                if (token == null
                    || !key.StartsWith(baseReference.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var relationship = key.Replace($"{baseReference.Name}:", string.Empty, StringComparison.InvariantCultureIgnoreCase);

                if (!_options.SupportedRelationships.Any(x => x.Equals(relationship, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                if (token is JArray)
                {
                    contentLinks.Add(GetRelationshipFrom(token, relationship, baseReference.Href));
                }
                else
                {
                    var child = token.ToObject<GraphItem>();
                    child.Uri = new Uri($"{baseReference.Href}{child.Href}");

                    contentLinks.Add(new GraphRelationship(relationship, new List<IGraphItem> { child }));
                }
            }

            return contentLinks;
        }

        internal GraphCurie GetBaseReference(JObject links) =>
            links?.GetValue<List<GraphCurie>>("curies")?.FirstOrDefault();

        internal IGraphRelation GetRelationshipFrom(JToken array, string relationshipKey, string baseHref)
        {
            var links = array.ToObject<List<GraphItem>>();

            links.ForEach(x => x.Uri = new Uri($"{baseHref}{x.Href}"));

            return new GraphRelationship(relationshipKey, links, TypeOfRelationship.Many);
        }
    }
}
