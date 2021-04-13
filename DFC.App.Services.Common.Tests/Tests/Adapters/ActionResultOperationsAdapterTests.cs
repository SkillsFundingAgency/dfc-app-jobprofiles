using DFC.App.Services.Common.Adapters;
using DFC.App.Services.Common.Factories;
using DFC.App.Services.Common.Faults;
using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Tests.Adapters
{
    [TestClass]
    public sealed class ActionResultOperationsAdapterTests :
        MoqTestingTests<ActionResultOperationsAdapter, IAdaptActionResultOperations>
    {
        [TestMethod]
        public void SupportsServiceRegistration()
        {
            // arrange / act / assert
            TestSystemSupportsGivenContract<IRequireServiceRegistration>();
        }

        [TestMethod]
        public void ConstructorWithNullSafeOperatorThrows()
        {
            // arrange
            var faultProvider = MakeStrictMock<IProvideFaultResponses>();
            var loggerFactory = MakeStrictMock<ICreateLoggingContexts>();

            // act / assert
            Assert.ThrowsException<ArgumentNullException>(() => new ActionResultOperationsAdapter(null, faultProvider, loggerFactory));
        }

        [TestMethod]
        public void ConstructorWithNullResponseProviderThrows()
        {
            // arrange
            var safeOperator = MakeStrictMock<IProvideSafeOperations>();
            var loggerFactory = MakeStrictMock<ICreateLoggingContexts>();

            // act / assert
            Assert.ThrowsException<ArgumentNullException>(() => new ActionResultOperationsAdapter(safeOperator, null, loggerFactory));
        }

        [TestMethod]
        public void ConstructorWithNullLoggerFactoryThrows()
        {
            // arrange
            var safeOperator = MakeStrictMock<IProvideSafeOperations>();
            var faultProvider = MakeStrictMock<IProvideFaultResponses>();

            // act / assert
            Assert.ThrowsException<ArgumentNullException>(() => new ActionResultOperationsAdapter(safeOperator, faultProvider, null));
        }

        [TestMethod]
        public async Task RunMeetsExpectation()
        {
            // arrange
            const string testMessage = "trace message";
            var response = MakeStrictMock<HttpResponseMessage>();
            async Task<HttpResponseMessage> TestFunc() => await Task.FromResult(response);

            var sut = BuildTestSystem();
            var scope = MakeStrictMock<ILoggingContextScope>();

            GetMock(scope)
                .Setup(x => x.Dispose());

            GetMock(sut.Logging)
                .Setup(x => x.BeginLoggingFor(testMessage))
                .Returns(scope);

            GetMock(sut.Operation)
                .Setup(x => x.Try(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<Func<Exception, Task<HttpResponseMessage>>>()))
                .Returns(Task.FromResult(response));

            // act
            var result = await sut.Run(TestFunc, testMessage);

            // assert
            VerifyAllMocks(sut);

            result.Should().BeAssignableTo<IActionResult>();
        }

        [TestMethod]
        [DataRow(typeof(MalformedRequestException), HttpStatusCode.BadRequest)]
        [DataRow(typeof(NoContentException), HttpStatusCode.AlreadyReported)]
        [DataRow(typeof(UnprocessableEntityException), HttpStatusCode.EarlyHints)]
        public async Task ProcessErrorMeetsExpectation(Type exceptionType, HttpStatusCode expectedResult)
        {
            // arrange
            var sut = BuildTestSystem();
            var scope = MakeStrictMock<ILoggingContextScope>();
            var exception = GetException(exceptionType);

            GetMock(sut.Faulted)
                .Setup(x => x.GetResponseFor(exception, scope))
                .Returns(Task.FromResult(new HttpResponseMessage(expectedResult)));

            // act
            var result = await sut.ProcessError(exception, scope);

            // assert
            VerifyAllMocks(sut);
            GetMock(scope).VerifyAll();

            result.Should().BeAssignableTo<HttpResponseMessage>();
            result.StatusCode.Should().Be(expectedResult);
        }

        internal Exception GetException(Type ofExeption, params string[] args) =>
            (Exception)Activator.CreateInstance(ofExeption, args);

        internal override void VerifyAllMocks(ActionResultOperationsAdapter sut)
        {
            GetMock(sut.Operation).VerifyAll();
            GetMock(sut.Faulted).VerifyAll();
            GetMock(sut.Logging).VerifyAll();
        }

        internal override ActionResultOperationsAdapter BuildTestSystem()
        {
            var safeOperator = MakeStrictMock<IProvideSafeOperations>();
            var faultProvider = MakeStrictMock<IProvideFaultResponses>();
            var loggerFactory = MakeStrictMock<ICreateLoggingContexts>();

            return new ActionResultOperationsAdapter(safeOperator, faultProvider, loggerFactory);
        }
    }
}
