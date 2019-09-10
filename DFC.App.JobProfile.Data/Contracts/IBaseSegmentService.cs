using DFC.App.JobProfile.Data.HttpClientPolicies;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IBaseSegmentService<TModel>
    {
        string CanonicalName { get; set; }

        SegmentClientOptions SegmentClientOptions { get; set; }

        Task<TModel> LoadDataAsync();

        Task<string> LoadMarkupAsync();
    }
}