using System.Collections.Generic;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public interface IGraphRelation
    {
        string Relationship { get; }

        TypeOfRelationship RelationshipType { get; }

        IReadOnlyCollection<IGraphItem> Items { get; }
    }
}
