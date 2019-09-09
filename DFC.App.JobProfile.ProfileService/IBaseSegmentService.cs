using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService
{
    public interface IBaseSegmentService<TModel>
    {
        Task<TModel> LoadAsync(string article);
    }
}