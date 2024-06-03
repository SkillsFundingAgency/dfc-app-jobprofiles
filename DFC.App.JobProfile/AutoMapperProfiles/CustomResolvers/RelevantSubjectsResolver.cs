using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.Helpers;
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

            if (source != null && source.JobProfileHowToBecome.IsAny())
            {
                var responseData = source.JobProfileHowToBecome.FirstOrDefault();

                switch (routeName)
                {
                    case RouteName.Apprenticeship:
                        relevantSubjects = responseData.ApprenticeshipRelevantSubjects.Html;
                        break;
                    case RouteName.College:
                        relevantSubjects = responseData.CollegeRelevantSubjects.Html;
                        break;
                    case RouteName.University:
                        relevantSubjects = responseData.UniversityRelevantSubjects.Html;
                        break;
                }
            }

            return relevantSubjects;
        }
    }
}
