using DFC.App.Services.Common.Faults;
using DFC.App.Services.Common.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Tests.Providers
{
    [TestClass]
    public sealed class SafeOperationsProviderTests :
        MoqTestingTests<SafeOperationsProvider, IProvideSafeOperations>
    {
        [TestMethod]
        [DataRow(typeof(MalformedRequestException))]
        [DataRow(typeof(NoContentException))]
        [DataRow(typeof(UnprocessableEntityException))]
        [DataRow(typeof(ArgumentNullException))]
        public async Task VoidTryWithExceptionMeetsExpectation(Type expectedException)
        {
            // arrange
            var sut = BuildTestSystem();
            var exception = (Exception)expectedException.Assembly.CreateInstance(expectedException.FullName);

            // act / assert
            await sut.Try(
                () => throw exception,
                x =>
                {
                    x.Should().BeAssignableTo(expectedException);
                    return Task.CompletedTask;
                });
        }

        [TestMethod]
        public async Task VoidTryMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();
            var threwError = false;
            var testItem = 1;

            // act / assert
            await sut.Try(
                () => Task.Run(() => testItem++),
                x =>
                {
                    threwError = true;
                    return Task.CompletedTask;
                });

            // assert
            testItem.Should().Be(2);
            threwError.Should().Be(false);
        }

        [TestMethod]
        [DataRow(0, 0, -1, true)] // this will throw divide by zero exception
        [DataRow(0, 1, 0, false)]
        [DataRow(1, 1, 1, false)]
        [DataRow(1, 0, -1, true)] // this will throw divide by zero exception
        public async Task TryWithResultMeetsExpectation(int first, int second, int expectedResult, bool expectedState)
        {
            // arrange
            var threwError = false;
            var sut = BuildTestSystem();

            // act
            var result = await sut.Try(
                () => Task.FromResult(first / second),
                x =>
                {
                    threwError = true;
                    return Task.FromResult(-1);
                });

            // assert
            result.Should().Be(expectedResult);
            threwError.Should().Be(expectedState);
        }

        internal override SafeOperationsProvider BuildTestSystem() =>
            new SafeOperationsProvider();

        internal override void VerifyAllMocks(SafeOperationsProvider sut)
        {
            // nothing to do...
        }
    }
}
