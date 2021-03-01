// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
using DFC.App.JobProfile.Data.Models;
using DFC.Compui.Cosmos.Contracts;

namespace DFC.App.JobProfile.Data.Providers
{
    internal sealed class CurrentOpportunitiesProvider :
        ContentPageProvider<CurrentOpportunities>,
        IProvideCurrentOpportunities
    {
        // sample current opportunity
        //private const string Candidate = @"{
        //    ""id"": ""f5e0bcba-d7b6-4831-be64-f8f828a6cc35"",
        //    ""Title"": ""'development sample'"",
        //    ""Apprenticeships"": [
        //        {
        //            ""Header"": {
        //                ""Text"": ""Apprentice Floorlayer"",
        //                ""Link"": ""https://www.findapprenticeship.service.gov.uk/apprenticeship/reference/1000010478""
        //            },
        //            ""Wage"": ""£156.00"",
        //            ""WageFrequency"": ""Weekly"",
        //            ""Location"": """"
        //        },
        //        {
        //            ""Header"": {
        //                ""Text"": ""Floorlaying Apprentice"",
        //                ""Link"": ""https://www.findapprenticeship.service.gov.uk/apprenticeship/reference/1671329""
        //                },
        //            ""Wage"": ""£156.00"",
        //            ""WageFrequency"": ""Weekly"",
        //            ""Location"": ""Wincanton""
        //        }
        //    ],
        //    ""Courses"": [
        //        {
        //            ""Header"": {
        //                ""Text"": ""Animal Care"",
        //                ""Link"": ""https://nationalcareers.service.gov.uk/find-a-course/course-details?courseid=cd46ac8c-2269-4360-9c9c-c47615f8aee1&r=fc76b744-5ca8-427e-a76e-e3aa353d4ecb""
        //            },
        //            ""Provider"": ""HAVANT AND SOUTH DOWNS COLLEGE"",
        //            ""StartDate"": ""06 September 2021"",
        //            ""Location"": ""Waterlooville""
        //        },
        //        {
        //            ""Header"": {
        //                ""Text"": ""Diploma Level 2 Horse Care"",
        //                ""Link"": ""https://nationalcareers.service.gov.uk/find-a-course/course-details?courseid=762b5b64-c786-4e70-8663-b43b0f1926b7&r=70c968b9-9124-4f22-ab38-5569cfb0d82c""
        //            },
        //            ""Provider"": ""MOULTON COLLEGE"",
        //            ""StartDate"": ""06 September 2021"",
        //            ""Location"": ""Northampton""
        //        }
        //    ],
        //    ""CanonicalName"": ""jobprofile-canonicalname"",
        //    ""PageLocation"": ""/Current-Opportunities""
        //}";

        public CurrentOpportunitiesProvider(
            IContentPageService<CurrentOpportunities> pageService)
            : base(pageService)
        {
            //pageService.UpsertAsync(JsonConvert.DeserializeObject<CurrentOpportunities>(Candidate));
        }
    }
}