namespace DFC.App.JobProfile.Data.Models.HowToBecome
{
    public sealed class RealStory
    {
        /// <summary>
        /// Gets or sets the title of the real story.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the summary text for of the real story.
        /// </summary>
        /// <value>
        /// Plain text.
        /// </value>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail of the real story.
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
        /// Gets or sets the body HTML content of the real story.
        /// </summary>
        /// <remarks>
        /// <para>This is raw HTML text that is input into a WYSIWYG field in the CMS.</para>
        /// </remarks>
        public string BodyHtml { get; set; }
    }
}
