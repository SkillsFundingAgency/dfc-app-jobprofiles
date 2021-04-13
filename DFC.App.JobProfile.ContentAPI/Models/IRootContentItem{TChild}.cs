using System.Collections.Generic;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public interface IRootContentItem<TChild> : IContainGraphLink
        where TChild : class, ILinkedContentItem<TChild>
    {
        ICollection<TChild> ContentItems { get; }

        bool IsFaultedState();
    }
}
