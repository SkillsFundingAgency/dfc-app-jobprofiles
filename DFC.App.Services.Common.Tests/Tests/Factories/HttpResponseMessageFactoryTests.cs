using DFC.App.Services.Common.Converters;
using DFC.App.Services.Common.Factories;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Tests.Factories
{
    [TestClass]
    public sealed class HttpResponseMessageFactoryTests :
        MoqTestingTests<HttpResponseMessageFactory, ICreateHttpResponseMessages>
    {
        [TestMethod]
        public void ConstructorFailsWithNullConverter()
        {
            // arrange / act /assert
            Assert.ThrowsException<ArgumentNullException>(() => new HttpResponseMessageFactory(null));
        }

        [TestMethod]
        [DataRow(HttpStatusCode.OK, "any old test content..")]
        [DataRow(HttpStatusCode.AlreadyReported, "more test content..")]
        [DataRow(HttpStatusCode.BadGateway, "even more test content..")]
        public async Task CreateWithStringMeetsExpectation(HttpStatusCode testCode, string testContent)
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = sut.Create(testCode, testContent);

            // assert
            VerifyAllMocks(sut);

            result.StatusCode.Should().Be(testCode);
            Assert.AreEqual(testContent, await result.Content.ReadAsStringAsync());
        }

        [TestMethod]
        [DataRow(HttpStatusCode.OK, "any old test content..")]
        [DataRow(HttpStatusCode.AlreadyReported, "more test content..")]
        [DataRow(HttpStatusCode.BadGateway, "even more test content..")]
        public async Task CreateWithTypeMeetsExpectation(HttpStatusCode testCode, string testContent)
        {
            // arrange
            var sut = BuildTestSystem();
            var item = new object();

            GetMock(sut.Convert)
                .Setup(x => x.ToString(item))
                .Returns(testContent);

            // act
            var result = await sut.Create(testCode, item);

            // assert
            VerifyAllMocks(sut);

            result.StatusCode.Should().Be(testCode);
            Assert.AreEqual(testContent, await result.Content.ReadAsStringAsync());
        }

        internal override HttpResponseMessageFactory BuildTestSystem()
        {
            var convert = MakeStrictMock<IConvertJsonTypes>();
            return new HttpResponseMessageFactory(convert);
        }

        internal override void VerifyAllMocks(HttpResponseMessageFactory sut)
        {
            GetMock(sut.Convert).VerifyAll();
        }
    }
}
