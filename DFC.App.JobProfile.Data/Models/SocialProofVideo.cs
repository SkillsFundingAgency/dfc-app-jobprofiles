namespace DFC.App.JobProfile.Data.Models
{
    public sealed class SocialProofVideo
    {
        /// <summary>
        /// Gets or sets the title of the social proof video. This is used to render the
        /// <c>title</c> attribute of a video embed.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the summary text for of the social proof video.
        /// </summary>
        /// <value>
        /// Plain text.
        /// </value>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the URL of the social proof video.
        /// </summary>
        /// <remarks>
        /// <para>At the time of writing this would be the URL of a video on YouTube.</para>
        /// </remarks>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the transcript text for the social proof video.
        /// </summary>
        /// <value>
        /// Plain text.
        /// </value>
        public string Transcript { get; set; }
    }
}
