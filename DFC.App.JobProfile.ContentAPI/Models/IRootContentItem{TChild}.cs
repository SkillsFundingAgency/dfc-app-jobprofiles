using System.Collections.Generic;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public interface IRootContentItem<TChild> :
        IContainGraphCuries
        where TChild : class, IBranchContentItem<TChild>
    {
        ICollection<TChild> ContentItems { get; }

        bool IsFaultedState();
    }
}
