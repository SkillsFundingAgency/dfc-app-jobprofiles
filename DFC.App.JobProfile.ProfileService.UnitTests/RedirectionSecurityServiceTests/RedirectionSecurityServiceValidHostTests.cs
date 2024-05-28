using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.RedirectionSecurityServiceTests
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
            IConfiguration config = SetupConfiguration();

            var redirectService = new ProfileService.RedirectionSecurityService(config);

            // act
            var result = redirectService.IsValidHost(new Uri(hostUrl));

            // assert
            Assert.Equal(expectedResult, result);
        }

        private static IConfiguration SetupConfiguration()
        {
            return new ConfigurationBuilder()
        .AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("AllowedRedirects", "nationalcareersservice, nationalcareers"),
        })
        .Build();
        }

        [Fact]
        public void RedirectionSecurityServiceShouldThrowNullExceptionWhenConfigurationIsNULLTest()
        {
            Assert.Throws<ArgumentNullException>(() => new ProfileService.RedirectionSecurityService(null));
        }

        [Fact]
        public void RedirectionSecurityServiceShouldThrowNullExceptionOnHostValueNULLTest()
        {
            IConfiguration config = SetupConfiguration();

            // act
            Assert.Throws<ArgumentNullException>(() => new ProfileService.RedirectionSecurityService(config).IsValidHost(null));
        }
    }
}