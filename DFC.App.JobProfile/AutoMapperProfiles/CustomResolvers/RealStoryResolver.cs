using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.Helpers;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class RealStoryResolver : IValueResolver<JobProfileHowToBecomeResponse, HowToBecomeSegmentDataModel, RealStory>
    {
        public RealStory Resolve(
            JobProfileHowToBecomeResponse source,
            HowToBecomeSegmentDataModel destination,
            RealStory destMember,
            ResolutionContext context)
        {
            RealStory realStory = null;

            if (source != null && source.JobProfileHowToBecome.IsAny())
            {
                var responseData = source.JobProfileHowToBecome.FirstOrDefault();

                if (responseData.RealStory != null && responseData.RealStory.ContentItems.IsAny())
                {
                    if (responseData.RealStory.ContentItems.IsAny())
                    {
                        var realStoryData = responseData.RealStory.ContentItems.FirstOrDefault();

                        realStory = new RealStory
                        {
                            Title = realStoryData.DisplayText,
                            Summary = realStoryData.Text,
                            BodyHtml = realStoryData.Body.Html,
                            FurtherInformationHtml = realStoryData.FurtherInformation.Html,
                            Thumbnail = new Thumbnail
                            {
                                Url = realStoryData.Thumbnail.Urls.FirstOrDefault(),
                                Text = realStoryData.Thumbnail.MediaText.FirstOrDefault(),
                            },
                        };
                    }
                }
            }

            return realStory;
        }
    }
}
