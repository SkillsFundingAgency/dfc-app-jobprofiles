using System.Collections.Generic;

namespace DFC.App.JobProfile.ProfileService.Utilities
{
    public interface IFormatContentService
    {
        string GetParagraphText(string openingText, IEnumerable<string> dataItems, string separator);
    }
}
