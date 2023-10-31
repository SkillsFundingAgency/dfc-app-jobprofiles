using DFC.App.JobProfile.Data.Enums;

namespace DFC.App.JobProfile.Data.Models
{
    public sealed class SocialProofVideo
    {
        /// <summary>
        /// Gets or sets the type of the social proof video.
        /// </summary>
        public SocialProofVideoType Type { get; set; }

        /// <summary>
        /// Gets or sets the title of the social proof video. This is used to render the
        /// <c>title</c> attribute of a video embed.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the summary HTML content for of the social proof video.
        /// </summary>
        /// <remarks>
        /// <para>This is raw HTML text that is input into a WYSIWYG field in the CMS.</para>
        /// </remarks>
        public string SummaryHtml { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail of the video.
        /// </summary>
        /// <value>
        /// A reference to the thumbnail image; otherwise, a value of <c>null</c>.
        /// </value>
        public Thumbnail Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets the further information HTML content that is shown below the thumbnail.
        /// </summary>
        /// <remarks>
        /// <para>This is raw HTML text that is input into a WYSIWYG field in the CMS.</para>
        /// </remarks>
        public string FurtherInformationHtml { get; set; }

        /// <summary>
        /// Gets or sets the URL of the social proof video.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the link text for showing a call to action button.
        /// </summary>
        /// <value>
        /// Plain text when call to action button is present; otherwise, a value of <c>null</c>.
        /// </value>
        public string LinkText { get; set; }

        /// <summary>
        /// Gets or sets the duration of the social proof video.
        /// </summary>
        /// <remarks>
        /// <para>This is text that is provided by the content editor; eg "One minute watch".</para>
        /// </remarks>
        public string Duration { get; set; }

        /// <summary>
        /// Gets or sets the transcript text for the social proof video.
        /// </summary>
        /// <value>
        /// Plain text.
        /// </value>
        public string Transcript { get; set; }
    }
}
