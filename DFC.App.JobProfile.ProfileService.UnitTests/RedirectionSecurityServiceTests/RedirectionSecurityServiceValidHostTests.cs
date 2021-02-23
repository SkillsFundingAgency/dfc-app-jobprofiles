using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.JobProfile.RedirectionSecurityService.UnitTests.RedirectionSecurityServiceTests
{
    [Trait("RedirectionSecurity Service", "Valid Host Tests")]
    public class RedirectionSecurityServiceTests
    {

        [Theory]
        [InlineData(false, "http://localhosttest")]
        [InlineData(true, "http://nationalcareers.co.uk")]
        [InlineData(true, "http://nationalcareersservice.co.uk")]
        [InlineData(false, "http://falseurl.in")]
        public void RedirectionSecurityServiceValidHostTests(bool expectedResult, string hostUrl)
        {
            //arrange
            IConfiguration config = new ConfigurationBuilder()
        .AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("AllowedRedirects:0", "nationalcareersservice"),
            new KeyValuePair<string, string>("AllowedRedirects:1", "nationalcareers"),
        })
        .Build();

            var redirectService = new ProfileService.RedirectionSecurityService(config);

            // act
            var result = redirectService.IsValidHost(new Uri(hostUrl));

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void RedirectionSecurityServiceShouldThrowNullExceptionWhenConfigurationIsNULLTest()
        {
            Assert.Throws<ArgumentNullException>(() => new ProfileService.RedirectionSecurityService(null));
        }

        [Fact]
        public void RedirectionSecurityServiceShouldThrowNullExceptionOnHostValueNULLTest()
        {
            IConfiguration config = new ConfigurationBuilder()
        .AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("AllowedRedirects:0", "nationalcareersservice"),
            new KeyValuePair<string, string>("AllowedRedirects:1", "nationalcareers"),
        })
        .Build();

            // act
            Assert.Throws<ArgumentNullException>(() => new ProfileService.RedirectionSecurityService(config).IsValidHost(null));
        }
    }
}