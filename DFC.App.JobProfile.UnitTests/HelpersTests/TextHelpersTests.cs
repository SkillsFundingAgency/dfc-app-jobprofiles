using DFC.App.JobProfile.Helpers;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.HelpersTests
{
    public sealed class TextHelpersTests
    {
        [Theory]
        [InlineData(
            "  Line 1  \r\n  Line 2  \r\n  Line 3  ",
            new string[]
            {
                "Line 1",
                "Line 2",
                "Line 3",
            }
        )]
        [InlineData(
            "  Line 1  \r  Line 2  \r  Line 3  ",
            new string[]
            {
                "Line 1",
                "Line 2",
                "Line 3",
            }
        )]
        [InlineData(
            "  Line 1  \n  Line 2  \n  Line 3  ",
            new string[]
            {
                "Line 1",
                "Line 2",
                "Line 3",
            }
        )]
        public void SplitTextIntoLines_TextWithLineBreaks_ReturnsExpectedLines(string text, string[] expectedLines)
        {
            // Arrange

            // Act
            var result = TextHelpers.SplitTextIntoLines(text);

            // Assert
            Assert.Equivalent(expectedLines, result);
        }

        [Fact]
        public void SplitTextIntoLines_NullText_ReturnsEmptyArray()
        {
            // Arrange

            // Act
            var result = TextHelpers.SplitTextIntoLines(null);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        public void SplitTextIntoLines_EmptyText_ReturnsEmptyArray(string text)
        {
            // Arrange

            // Act
            var result = TextHelpers.SplitTextIntoLines(text);

            // Assert
            Assert.Empty(result);
        }
    }
}
