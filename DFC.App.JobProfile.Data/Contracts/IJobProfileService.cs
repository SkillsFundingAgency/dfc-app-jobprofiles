using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IJobProfileService
    {
        Task<IEnumerable<JobProfileModel>> GetAllAsync();

        Task<JobProfileModel> GetByNameAsync(string canonicalName);

        Task<SegmentModel> GetHowToBecomeSegmentAsync(string canonicalName, string filter);

        Task<SegmentModel> GetOverviewSegment(string canonicalName, string filter);

        Task<SegmentModel> GetRelatedCareersSegmentAsync(string canonicalName, string filter);

        Task<SegmentModel> GetCareerPathSegmentAsync(string canonicalName, string filter);

        Task<SegmentModel> GetCurrentOpportunities(string canonicalName);

        Task<SegmentModel> GetSkillSegmentAsync(string canonicalName, string filter);

        Task<SocialProofVideo> GetSocialProofVideoSegment(string canonicalName, string filter);

        Task<SegmentModel> GetTasksSegmentAsync(string canonicalName, string filter);

        Task<bool> RefreshCourses(string filter, int first, int skip);

        Task<bool> RefreshApprenticeshipsAsync(string filter, int first, int skip);

        Task<bool> RefreshAllSegments(string filter, int first, int skip);
    }
}