using DFC.App.JobProfile.ProfileService.Utilities;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.Utilities
{
    public class FormatContentServiceTests
    {
        private const string OpeningText = "openingText1";
        private const string Separator = ",";

        [Fact]
        public void WhenDataItemsAreEmptyThenEmptyStringReturned()
        {
            // Arrange
            var service = new FormatContentService();
            var items = new List<string>();

            // Act
            var result = service.GetParagraphText(OpeningText, items, Separator);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void JoinSingleItemsUsingSeparator()
        {
            // Arrange
            var service = new FormatContentService();
            var items = new List<string> { "Item1" };

            // Act
            var result = service.GetParagraphText(OpeningText, items, Separator);

            // Assert
            Assert.Equal("openingText1 Item1.", result);
        }

        [Fact]
        public void Join2ItemsUsingSeparator()
        {
            // Arrange
            var service = new FormatContentService();
            var items = new List<string> { "Item1", "Item2" };

            // Act
            var result = service.GetParagraphText(OpeningText, items, Separator);

            // Assert
            Assert.Equal("openingText1 Item1 , Item2.", result);
        }

        [Fact]
        public void JoinMoreThan2ItemsItemsUsingSeparator()
        {
            // Arrange
            var service = new FormatContentService();
            var items = new List<string> { "Item1", "Item2", "Item3" };

            // Act
            var result = service.GetParagraphText(OpeningText, items, Separator);

            // Assert
            Assert.Equal("openingText1 Item1, Item2 , Item3.", result);
        }
    }
}
