using AutoMapper;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.Helpers;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class RegistrationResolver : IValueResolver<JobProfileHowToBecomeResponse, HowToBecomeSegmentDataModel, List<Registration>>
    {
        public List<Registration> Resolve(
            JobProfileHowToBecomeResponse source,
            HowToBecomeSegmentDataModel destination,
            List<Registration> destMember,
            ResolutionContext context)
        {
            var registrations = new List<Registration>();

            if (source != null && source.JobProfileHowToBecome.IsAny())
            {
                var responseData = source.JobProfileHowToBecome.FirstOrDefault();

                if (responseData.RelatedRegistrations.ContentItems.IsAny())
                {
                    foreach (var item in responseData.RelatedRegistrations.ContentItems)
                    {
                        registrations.Add(new Registration
                        {
                            Title = item.DisplayText,
                            Description = item.Info.Html,
                        });
                    }
                }
            }

            return registrations;
        }
    }
}
