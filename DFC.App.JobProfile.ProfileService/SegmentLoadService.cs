using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models.Segments;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService
{
    public class SegmentLoadService<T> : ISegmentLoadService<T>
        where T : BaseSegmentModel, new()
    {
        public async Task<T> LoadAsync()
        {
            var result = new T
            {
                LastReviewed = DateTime.UtcNow,
                Content = $"<h1>{typeof(T).Name}</h1><p>hello world</p>",
            };

            return result;
        }
    }
}
