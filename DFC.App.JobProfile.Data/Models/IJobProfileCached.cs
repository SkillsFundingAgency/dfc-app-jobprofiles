using DFC.App.JobProfile.ContentAPI.Models;
using DFC.Compui.Cosmos.Contracts;

namespace DFC.App.JobProfile.Data.Models
{
    public interface IJobProfileCached :
        IContentPageModel,
        IResourceLocatable
    {
    }
}
