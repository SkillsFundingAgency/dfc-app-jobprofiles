using System.Text.RegularExpressions;

namespace DFC.App.JobProfile.Helpers
{
    public static class VideoHelpers
    {
        private static Regex videoIdRegex = new Regex(
            pattern: @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/ ]{11})",
            options: RegexOptions.Compiled
        );

        /// <summary>
        /// Extracts the unique video identifier from a given YouTube URL.
        /// </summary>
        /// <remarks>
        /// <para>The following patterns are recognised:</para>
        /// <list type="bullet">
        ///   <item>https://www.youtube.com/watch?v={videoId}</item>
        ///   <item>https://youtube.com/v/{videoId}</item>
        ///   <item>https://youtube.com/embed/{videoId}</item>
        ///   <item>https://youtube.com/e/{videoId}</item>
        ///   <item>https://youtu.be/{videoId}?xyz=123</item>
        /// </list>
        /// </remarks>
        /// <param name="youtubeUrl">A YouTube URL.</param>
        /// <returns>
        /// The extracted video identifier; otherwise, a value of <c>null</c> if URL was not recognised.
        /// </returns>
        public static string ExtractVideoIdFromYoutubeUrl(string youtubeUrl)
        {
            if (!string.IsNullOrEmpty(youtubeUrl))
            {
                var videoIdMatch = videoIdRegex.Match(youtubeUrl);
                if (videoIdMatch.Success)
                {
                    return videoIdMatch.Groups[1].Value;
                }
            }
            return null;
        }
    }
}
