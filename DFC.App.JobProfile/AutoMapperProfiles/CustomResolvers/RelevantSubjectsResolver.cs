using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.HowToBecome;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class RelevantSubjectsResolver : IValueResolver<JobProfileHowToBecomeResponse, CommonRoutes, string>
    {
        public string Resolve(JobProfileHowToBecomeResponse source, CommonRoutes destination, string destMember, ResolutionContext context)
        {
            RouteName routeName = (RouteName)context.Items["RouteName"];
            var relevantSubjects = string.Empty;

            switch (routeName)
            {
                case RouteName.Apprenticeship:
                    relevantSubjects = source.JobProfileHowToBecome.FirstOrDefault()?.Apprenticeshiprelevantsubjects.Html;
                    break;
                case RouteName.College:
                    relevantSubjects = source.JobProfileHowToBecome.FirstOrDefault()?.Collegerelevantsubjects.Html;
                    break;
                case RouteName.University:
                    relevantSubjects = source.JobProfileHowToBecome.FirstOrDefault()?.Universityrelevantsubjects.Html;
                    break;
            }

            return relevantSubjects;
        }
    }
}
