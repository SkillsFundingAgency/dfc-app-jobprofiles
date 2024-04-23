using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class EntryRequirementsResolver : IValueResolver<JobProfileHowToBecomeResponse, CommonRoutes, List<EntryRequirement>>
    {
        public List<EntryRequirement> Resolve(
            JobProfileHowToBecomeResponse source,
            CommonRoutes destination,
            List<EntryRequirement> destMember,
            ResolutionContext context)
        {
            RouteName routeName = (RouteName)context.Items["RouteName"];
            var entryRequirements = new List<EntryRequirement>();

            if (source.JobProfileHowToBecome != null)
            {
                var responseData = source.JobProfileHowToBecome.FirstOrDefault();

                switch (routeName)
                {
                    case RouteName.Apprenticeship:
                        if (responseData.RelatedApprenticeshipRequirements.ContentItems.Any() &&
                            responseData.RelatedApprenticeshipRequirements.ContentItems.FirstOrDefault().Info != null)
                        {
                            foreach (var item in responseData.RelatedApprenticeshipRequirements.ContentItems)
                            {
                                entryRequirements.Add(new EntryRequirement
                                {
                                    Description = item.Info.Html,
                                });
                            }
                        }

                        break;
                    case RouteName.College:
                        if (responseData.RelatedCollegeRequirements.ContentItems.Any() &&
                            responseData.RelatedCollegeRequirements.ContentItems.FirstOrDefault().Info != null)
                        {
                            foreach (var item in responseData.RelatedCollegeRequirements.ContentItems)
                            {
                                entryRequirements.Add(new EntryRequirement
                                {
                                    Description = item.Info.Html,
                                });
                            }
                        }

                        break;
                    case RouteName.University:
                        if (responseData.RelatedUniversityRequirements.ContentItems.Any() &&
                            responseData.RelatedUniversityRequirements.ContentItems.FirstOrDefault().Info != null)
                        {
                            foreach (var item in responseData.RelatedUniversityRequirements.ContentItems)
                            {
                                entryRequirements.Add(new EntryRequirement
                                {
                                    Description = item.Info.Html,
                                });
                            }
                        }

                        break;
                }
            }

            return entryRequirements;
        }
    }
}