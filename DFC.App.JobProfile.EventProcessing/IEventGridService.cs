using DFC.App.JobProfile.EventProcessing.Models;
using DFC.Compui.Cosmos.Contracts;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.EventProcessing
{
    public interface IEventGridService<in TModel>
        where TModel : class, IContentPageModel
    {
        Task CompareThenSendEvent(TModel currentModel, TModel changedModel);

        Task SendEvent(EventOperation operation, TModel changedModel);
    }
}
