using AutoMapper;
using DFC.App.JobProfile.Data.Models.Segment.Tasks;
using DFC.App.JobProfile.Helpers;
using DFC.App.JobProfile.ProfileService.Utilities;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class LocationResolver : FormatContentService, IValueResolver<JobProfileWhatYoullDoResponse, TasksSegmentDataModel, string>
    {
        public string Resolve(
            JobProfileWhatYoullDoResponse source,
            TasksSegmentDataModel destination,
            string destMember,
            ResolutionContext context)
        {
            var wordList = new List<string>();

            if (source.JobProfileWhatYoullDo.IsAny())
            {
                var responseData = source.JobProfileWhatYoullDo.FirstOrDefault();

                if (responseData.RelatedLocations.ContentItems.IsAny())
                {
                    foreach (var contentItem in responseData.RelatedLocations.ContentItems)
                    {
                        wordList.Add(contentItem.Description);
                    }

                    return Convert(wordList);
                }
            }

            return string.Empty;
        }

        public string Convert(IEnumerable<string> wordList)
        {
            const string LocationLeadingText = "You could work";
            const string LocationConjunction = "or";

            return GetParagraphText(LocationLeadingText, wordList, LocationConjunction);
        }
    }
}
