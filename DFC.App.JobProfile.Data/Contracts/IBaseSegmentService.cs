using DFC.App.JobProfile.Data.HttpClientPolicies;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IBaseSegmentService<TModel>
    {
        SegmentClientOptions SegmentClientOptions { get; set; }

        Task<TModel> LoadAsync(string article);
    }
}