using DFC.App.JobProfile.Helpers;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.HelpersTests
{
    public sealed class VideoHelpersTests
    {
        [Theory]
        [InlineData("https://www.youtube.com/watch?v=123456xTeSt", "123456xTeSt")]
        [InlineData("https://www.youtube.com/watch?v=123456xTeSt?abc=123&def=456", "123456xTeSt")]
        [InlineData("https://youtube.com/v/123456xTeSt", "123456xTeSt")]
        [InlineData("https://youtube.com/v/123456xTeSt?abc=123&def=456", "123456xTeSt")]
        [InlineData("https://youtube.com/embed/123456xTeSt", "123456xTeSt")]
        [InlineData("https://youtube.com/embed/123456xTeSt?abc=123&def=456", "123456xTeSt")]
        [InlineData("https://youtube.com/e/123456xTeSt", "123456xTeSt")]
        [InlineData("https://youtube.com/e/123456xTeSt?abc=123&def=456", "123456xTeSt")]
        [InlineData("https://youtu.be/123456xTeSt", "123456xTeSt")]
        [InlineData("https://youtu.be/123456xTeSt?abc=123&def=456", "123456xTeSt")]
        public void ExtractVideoIdFromYoutubeUrl_ValidYoutubeUrl_ReturnsExpectedVideoId(string youtubeUrl, string expectedVideoId)
        {
            // Arrange

            // Act
            var result = VideoHelpers.ExtractVideoIdFromYoutubeUrl(youtubeUrl);

            // Assert
            Assert.Equal(expectedVideoId, result);
        }

        [Theory]
        [InlineData("https://www.youtube.com/watch?foo=123456xTeSt")]
        [InlineData("https://www.gov.uk/watch?v=123456xTeSt")]
        [InlineData("https://gov.uk/v/123456xTeSt")]
        [InlineData("https://gov.uk/embed/123456xTeSt")]
        [InlineData("https://gov.uk/e/123456xTeSt")]
        public void ExtractVideoIdFromYoutubeUrl_InvalidYoutubeUrl_ReturnsNull(string invalidUrl)
        {
            // Arrange

            // Act
            var result = VideoHelpers.ExtractVideoIdFromYoutubeUrl(invalidUrl);

            // Assert
            Assert.Null(result);
        }
    }
}
