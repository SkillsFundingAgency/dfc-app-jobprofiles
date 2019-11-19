using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ISegmentLoadService<T>
    {
        Task<T> LoadAsync();
    }
}