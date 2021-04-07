using System.Collections.Generic;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    internal sealed class GraphRelationship :
        IGraphRelation
    {
        public GraphRelationship(string relationship, IReadOnlyCollection<IGraphItem> items, TypeOfRelationship type = TypeOfRelationship.One)
        {
            Relationship = relationship;
            RelationshipType = type;
            Items = items;
        }

        public string Relationship { get; }

        public TypeOfRelationship RelationshipType { get; }

        public IReadOnlyCollection<IGraphItem> Items { get; }
    }
}
