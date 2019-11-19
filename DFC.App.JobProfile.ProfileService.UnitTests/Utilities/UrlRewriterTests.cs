using DFC.App.JobProfile.ProfileService.Utilities;
using FakeItEasy;
using System;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.Utilities
{
    [Trait("UrlRewriter Utility", "Content URL Rewriter Tests")]
    public class UrlRewriterTests
    {
        public static IEnumerable<object[]> ContentLinkMappings => new List<object[]>
        {
            new object[] { "/", "http://change-to-this" },
            new object[] { "/abc", "http://change-to-this/abc" },
            new object[] { "http://change-from-this", "http://change-to-this" },
            new object[] { "https://change-from-this", "http://change-to-this" },
            new object[] { "http://change-from-this", "https://change-to-this" },
            new object[] { "https://change-from-this", "https://change-to-this" },
            new object[] { "http://change-from-this:8080", "http://change-to-this" },
            new object[] { "http://change-from-this", "http://change-to-this:8080" },
            new object[] { "http://change-from-this:8080", "http://change-to-this:8080" },
        };

        [Theory]
        [MemberData(nameof(ContentLinkMappings))]
        public void UrlRewriterUtilityReturnsSuccessForAnchorLinkDoubleQuote(string fromUrl, string toUrl)
        {
            // arrange
            string content = $"<div><a href=\"http://no-change/abcdef\">linky</a><a href=\"{fromUrl}/abcdef\"></div>";
            string expectedResult = $"<div><a href=\"http://no-change/abcdef\">linky</a><a href=\"{toUrl}/abcdef\"></div>";

            // act
            var result = UrlRewriter.Rewrite(content, new Uri(fromUrl, UriKind.RelativeOrAbsolute), new Uri(toUrl, UriKind.Absolute));

            // assert
            A.Equals(result, expectedResult);
        }

        [Theory]
        [MemberData(nameof(ContentLinkMappings))]
        public void UrlRewriterUtilityReturnsSuccessForAnchorLinkSingleQuote(string fromUrl, string toUrl)
        {
            // arrange
            string content = $"<div><a href='http://no-change/abcdef'>linky</a><a href='{fromUrl}/abcdef'></div>";
            string expectedResult = $"<div><a href='http://no-change/abcdef'>linky</a><a href='{toUrl}/abcdef'></div>";

            // act
            var result = UrlRewriter.Rewrite(content, new Uri(fromUrl, UriKind.RelativeOrAbsolute), new Uri(toUrl, UriKind.Absolute));

            // assert
            A.Equals(result, expectedResult);
        }

        [Theory]
        [MemberData(nameof(ContentLinkMappings))]
        public void UrlRewriterUtilityReturnsSuccessForFormActionDoubleQuote(string fromUrl, string toUrl)
        {
            // arrange
            string content = $"<div><form action=\"http://no-change/abcdef\" /><form action=\"{fromUrl}/abcdef\" /></div>";
            string expectedResult = $"<div><form action=\"http://no-change/abcdef\" /><form action=\"{toUrl}/abcdef\" /></div>";

            // act
            var result = UrlRewriter.Rewrite(content, new Uri(fromUrl, UriKind.RelativeOrAbsolute), new Uri(toUrl, UriKind.Absolute));

            // assert
            A.Equals(result, expectedResult);
        }

        [Theory]
        [MemberData(nameof(ContentLinkMappings))]
        public void UrlRewriterUtilityReturnsSuccessForFormActionSingleQuote(string fromUrl, string toUrl)
        {
            // arrange
            string content = $"<div><form action='http://no-change/abcdef' /><form action='{fromUrl}/abcdef' /></div>";
            string expectedResult = $"<div><form action='http://no-change/abcdef' /><form action='{toUrl}/abcdef' /></div>";

            // act
            var result = UrlRewriter.Rewrite(content, new Uri(fromUrl, UriKind.RelativeOrAbsolute), new Uri(toUrl, UriKind.Absolute));

            // assert
            A.Equals(result, expectedResult);
        }
    }
}