using System.Collections.Generic;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public interface IBranchContentItem<TModel> :
        IResourceLocatable,
        IContainGraphCuries
            where TModel : class
    {
        ICollection<TModel> ContentItems { get; }

        bool IsFaultedState();
    }
}
