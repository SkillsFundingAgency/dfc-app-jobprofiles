using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.Helpers;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class EntryRequirementsPrefaceResolver : IValueResolver<JobProfileHowToBecomeResponse, CommonRoutes, string>
    {
        public string Resolve(
            JobProfileHowToBecomeResponse source,
            CommonRoutes destination,
            string destMember,
            ResolutionContext context)
        {
            RouteName routeName = (RouteName)context.Items["RouteName"];
            string entryRequirements = null;

            if (source != null && source.JobProfileHowToBecome.IsAny())
            {
                var responseData = source.JobProfileHowToBecome.FirstOrDefault();

                switch (routeName)
                {
                    case RouteName.Apprenticeship:
                        entryRequirements = responseData.ApprenticeshipEntryRequirements.ContentItems.FirstOrDefault()?.DisplayText;
                        break;
                    case RouteName.College:
                        entryRequirements = responseData.CollegeEntryRequirements.ContentItems.FirstOrDefault()?.DisplayText;
                        break;
                    case RouteName.University:
                        entryRequirements = responseData.UniversityEntryRequirements.ContentItems.FirstOrDefault()?.DisplayText;
                        break;
                }
            }

            return entryRequirements;
        }
    }
}