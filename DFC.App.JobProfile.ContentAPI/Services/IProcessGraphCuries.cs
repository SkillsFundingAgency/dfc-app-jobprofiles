using DFC.App.JobProfile.ContentAPI.Models;
using System.Collections.Generic;

namespace DFC.App.JobProfile.ContentAPI.Services
{
    public interface IProcessGraphCuries
    {
        IReadOnlyCollection<IGraphRelation> GetContentItemLinkedItems(IContainGraphLink container);
    }
}
