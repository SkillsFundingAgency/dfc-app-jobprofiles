using DFC.App.Services.Common.Converters;
using DFC.App.Services.Common.Factories;
using DFC.App.Services.Common.Faults;
using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration.Attributes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Tests.Providers
{
    /// <summary>
    /// the fault response provider fixture.
    /// </summary>
    [TestClass]
    public sealed class FaultResponseProviderTests :
        MoqTestingTests<FaultResponseProvider, IProvideFaultResponses>
    {
        [TestMethod]
        [DataRow(typeof(UnprocessableEntityException), HttpStatusCode.UnprocessableEntity, "")]
        [DataRow(typeof(MalformedRequestException), HttpStatusCode.BadRequest, "")]
        [DataRow(typeof(ConflictingResourceException), HttpStatusCode.Conflict, "Resource already exists")]
        [DataRow(typeof(NoContentException), HttpStatusCode.NoContent, "Resource does not exist")]
        public async Task GetResponseForUsingFallbackMeetsExpectation(Type testException, HttpStatusCode expectedState, string expectedMessage)
        {
            // arrange
            var sut = BuildTestSystem();

            var assembly = GetAssemblyfor<FaultResponseProvider>();
            GetMock(sut.Details)
                .Setup(x => x.Load())
                .Returns(MakeReadonlyItems<Assembly>(1, assembly));
            var attribute = new FaultResponseRegistrationAttribute(testException, expectedState);
            GetMock(sut.Details)
                .Setup(x => x.GetAttributeListFor<FaultResponseRegistrationAttribute>(assembly))
                .Returns(MakeReadonlyItems<FaultResponseRegistrationAttribute>(1, attribute));

            GetMock(sut.Response)
                .Setup(x => x.Create(expectedState, expectedMessage))
                .Returns(new HttpResponseMessage(expectedState));

            var exception = (Exception)testException.Assembly.CreateInstance(testException.FullName);
            var logger = MakeLoggingContext(expectedMessage);

            // act
            var result = await sut.GetResponseFor(exception, logger);

            // assert
            VerifyAllMocks(sut);
            GetMock(logger).VerifyAll();

            result.StatusCode.Should().Be(expectedState);
        }

        [TestMethod]
        [DataRow(typeof(UnprocessableEntityException), HttpStatusCode.UnprocessableEntity, "")]
        [DataRow(typeof(MalformedRequestException), HttpStatusCode.BadRequest, "")]
        [DataRow(typeof(ConflictingResourceException), HttpStatusCode.Conflict, "Resource already exists")]
        [DataRow(typeof(NoContentException), HttpStatusCode.NoContent, "Resource does not exist")]
        public async Task RealSystem_GetResponseForUsingFallbackMeetsExpectation(Type testException, HttpStatusCode expectedState, string expectedMessage)
        {
            // arrange
            var sut = BuildRealSystem();

            var exception = (Exception)testException.Assembly.CreateInstance(testException.FullName);
            var logger = MakeLoggingContext(expectedMessage);

            // act
            var result = await sut.GetResponseFor(exception, logger);

            // assert
            GetMock(logger).VerifyAll();

            result.StatusCode.Should().Be(expectedState);

            // doesn't support aynchronous assertions
            // await result.Content.ReadAsStringAsync().Should().Be(expectedMessage);
            Assert.AreEqual(expectedMessage, await result.Content.ReadAsStringAsync());
        }

        [TestMethod]
        [DataRow(typeof(ArgumentNullException), "Value cannot be null.")]
        [DataRow(typeof(ArgumentException), "Value does not fall within the expected range.")]
        [DataRow(typeof(InvalidOperationException), "Operation is not valid due to the current state of the object.")]
        public async Task GetResponseForUnknownExceptionMeetsExpectation(Type testException, string expectedMessage)
        {
            // arrange
            var sut = BuildTestSystem();
            var assembly = GetAssemblyfor<FaultResponseProvider>();
            GetMock(sut.Details)
                .Setup(x => x.Load())
                .Returns(MakeReadonlyItems<Assembly>(1, assembly));
            var attribute = new FaultResponseRegistrationAttribute(typeof(NoContentException), HttpStatusCode.NoContent);
            GetMock(sut.Details)
                .Setup(x => x.GetAttributeListFor<FaultResponseRegistrationAttribute>(assembly))
                .Returns(MakeReadonlyItems<FaultResponseRegistrationAttribute>(1, attribute));
            GetMock(sut.Response)
                .Setup(x => x.Create(HttpStatusCode.BadRequest, expectedMessage))
                .Returns(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var exception = (Exception)testException.Assembly.CreateInstance(testException.FullName);
            var logger = MakeLoggingContext(null);
            GetMock(logger)
                .Setup(x => x.Log(exception))
                .Returns(Task.CompletedTask);

            // act
            var result = await sut.GetResponseFor(exception, logger);

            // assert
            VerifyAllMocks(sut);
            GetMock(logger).VerifyAll();

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        [DataRow(typeof(ArgumentNullException), "Value cannot be null.")]
        [DataRow(typeof(ArgumentException), "Value does not fall within the expected range.")]
        [DataRow(typeof(InvalidOperationException), "Operation is not valid due to the current state of the object.")]
        public async Task RealSystem_GetResponseForUnknownExceptionMeetsExpectation(Type testException, string expectedMessage)
        {
            // arrange
            var sut = BuildRealSystem();

            var exception = (Exception)testException.Assembly.CreateInstance(testException.FullName);
            var logger = MakeLoggingContext(null);
            GetMock(logger)
                .Setup(x => x.Log(exception))
                .Returns(Task.CompletedTask);

            // act
            var result = await sut.GetResponseFor(exception, logger);

            // assert
            GetMock(logger).VerifyAll();

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // doesn't support aynchronous assertions
            // await result.Content.ReadAsStringAsync().Should().Be(expectedMessage);
            Assert.AreEqual(expectedMessage, await result.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public void RealSystem_LoadedMappingMeetsExpectation()
        {
            // arrange
            var sut = BuildRealSystem();

            // act
            sut.Load();

            // assert
            // this count will be one more than the assembly fault registrations, as the fallback is a manual mapping.
            sut.Map.Count.Should().Be(5);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(3)]
        [DataRow(17)]
        public async Task InformOnMeetsExpectation(int testDepth)
        {
            // arrange
            const string message = "Resource already exists";
            var sut = BuildTestSystem();

            var logger = MakeLoggingContext(message);

            // act
            await sut.InformOn(GetException(testDepth), logger);

            // assert
            GetMock(logger).Verify(x => x.Log(message), Times.Exactly(testDepth));
        }

        internal ILoggingContextScope MakeLoggingContext(string itemBeingRecorded)
        {
            var logger = MakeStrictMock<ILoggingContextScope>();

            if (itemBeingRecorded != null)
            {
                GetMock(logger)
                    .Setup(x => x.Log(itemBeingRecorded))
                    .Returns(Task.CompletedTask);
            }

            return logger;
        }

        internal Exception GetException(int currentDepth)
        {
            var newDepth = --currentDepth;

            if (newDepth == 0)
            {
                return new ConflictingResourceException();
            }

            return new ConflictingResourceException("'issue unknown'", GetException(newDepth));
        }

        internal override void VerifyAllMocks(FaultResponseProvider sut)
        {
            GetMock(sut.Details).VerifyAll();
            GetMock(sut.Response).VerifyAll();
        }

        internal override FaultResponseProvider BuildTestSystem()
        {
            var details = MakeStrictMock<IProvideRegistrationDetails>();
            var factory = MakeStrictMock<ICreateHttpResponseMessages>();

            return new FaultResponseProvider(details, factory);
        }

        internal FaultResponseProvider BuildRealSystem()
        {
            var assets = new AssetProvider();
            var converter = new JsonTypeConverter();
            var details = new RegistrationDetailProvider(assets, converter);

            var factory = new HttpResponseMessageFactory(converter);

            return new FaultResponseProvider(details, factory);
        }
    }
}
