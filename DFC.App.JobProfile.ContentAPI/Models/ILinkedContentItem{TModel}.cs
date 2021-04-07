using System.Collections.Generic;

namespace DFC.App.JobProfile.ContentAPI.Models
{
    public interface ILinkedContentItem<TModel> :
        IResourceLocatable,
        IContainGraphLink
            where TModel : class
    {
        ICollection<TModel> ContentItems { get; }

        bool IsFaultedState();
    }
}
