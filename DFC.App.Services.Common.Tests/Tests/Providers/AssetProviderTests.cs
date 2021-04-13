using DFC.App.Services.Common.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Tests.Providers
{
    [TestClass]
    public sealed class AssetProviderTests :
        MoqTestingTests<AssetProvider, IProvideAssets>
    {
        [TestMethod]
        public async Task MissingAssetThrows()
        {
            // arrange
            var sut = BuildTestSystem();

            // act / assert
            await Assert.ThrowsExceptionAsync<FileNotFoundException>(() => sut.GetAsset("missing_asset_text_test.txt"));
        }

        [TestMethod]
        public async Task GetAssetMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = await sut.GetAsset("asset_text_test.txt");

            // assert
            result.Should().BeAssignableTo<Stream>();
        }

        [TestMethod]
        public async Task GetAssetTextMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = await sut.GetTextAsset("asset_text_test.txt");

            // assert
            result.Should().Be("this is an asset test text file...");
        }

        [TestMethod]
        public void GetAssetFileMeetsExpectation()
        {
            // arrange
            string sep = $"{Path.DirectorySeparatorChar}";
            var sut = BuildTestSystem();

            // act
            var result = sut.GetAssetFile("asset_text_test.txt");

            // assert
            result.Should().Contain(AppDomain.CurrentDomain.BaseDirectory);
            result.Should().EndWith($"{sep}Assets{sep}asset_text_test.txt");
        }

        internal override AssetProvider BuildTestSystem() =>
            new AssetProvider();

        internal override void VerifyAllMocks(AssetProvider sut)
        {
            // nothing to do..
        }
    }
}