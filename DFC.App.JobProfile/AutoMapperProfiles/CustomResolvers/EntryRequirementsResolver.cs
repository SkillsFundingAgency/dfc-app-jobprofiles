using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.HowToBecome;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class EntryRequirementsResolver : IValueResolver<JobProfileHowToBecomeResponse, CommonRoutes, List<EntryRequirement>>
    {
        public List<EntryRequirement> Resolve(JobProfileHowToBecomeResponse source, CommonRoutes destination, List<EntryRequirement> destMember, ResolutionContext context)
        {
            RouteName routeName = (RouteName)context.Items["RouteName"];
            var responseData = source.JobProfileHowToBecome.FirstOrDefault();
            var entryRequirements = new List<EntryRequirement>();

            switch (routeName)
            {
                case RouteName.Apprenticeship:
                    if (responseData.RelatedApprenticeshipRequirements.ContentItems.FirstOrDefault().Info != null)
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
                    if (responseData.RelatedCollegeRequirements.ContentItems.FirstOrDefault().Info != null)
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
                    if (responseData.UniversityEntryRequirements.ContentItems.FirstOrDefault().Info != null)
                    {
                        foreach (var item in responseData.UniversityEntryRequirements.ContentItems)
                        {
                            entryRequirements.Add(new EntryRequirement
                            {
                                Description = item.Info.Html,
                            });
                        }
                    }

                    break;
            }

            return entryRequirements;
        }
    }
}