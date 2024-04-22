using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class RelevantSubjectsResolver : IValueResolver<JobProfileHowToBecomeResponse, CommonRoutes, string>
    {
        public string Resolve(
            JobProfileHowToBecomeResponse source,
            CommonRoutes destination,
            string destMember,
            ResolutionContext context)
        {
            RouteName routeName = (RouteName)context.Items["RouteName"];
            string relevantSubjects = null;

            switch (routeName)
            {
                case RouteName.Apprenticeship:
                    relevantSubjects = source.JobProfileHowToBecome.FirstOrDefault()?.ApprenticeshipRelevantSubjects.Html;
                    break;
                case RouteName.College:
                    relevantSubjects = source.JobProfileHowToBecome.FirstOrDefault()?.CollegeRelevantSubjects.Html;
                    break;
                case RouteName.University:
                    relevantSubjects = source.JobProfileHowToBecome.FirstOrDefault()?.UniversityRelevantSubjects.Html;
                    break;
            }

            return relevantSubjects;
        }
    }
}
