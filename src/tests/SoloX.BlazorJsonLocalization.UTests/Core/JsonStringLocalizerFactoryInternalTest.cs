// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerFactoryInternalTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.UTests.Samples.Extension;
using SoloX.CodeQuality.Test.Helpers.XUnit.Logger;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class JsonStringLocalizerFactoryInternalTest
    {
        private const string CultureName = "en-us";

        private static readonly string BaseName = typeof(Component).FullName ?? nameof(Component);
        private static readonly Assembly Assembly = typeof(Component).Assembly;
        private static readonly CultureInfo CultureInfo = CultureInfo.GetCultureInfo(CultureName);

        private readonly Mock<IStringLocalizerInternal> localizer;
        private readonly JsonStringLocalizerFactory factory;
        private readonly Mock<IJsonLocalizationExtensionService> extensionServiceMock;
        private readonly ExtensionOptionsContainer<MyOptions> extensionOptionsContainer;

        private ILogger<JsonStringLocalizerFactory> Logger { get; }
        private ILogger<StringLocalizerProxy> LoggerForStringLocalizerProxy { get; }

        public JsonStringLocalizerFactoryInternalTest(ITestOutputHelper testOutputHelper)
        {
            Logger = new TestLogger<JsonStringLocalizerFactory>(testOutputHelper);
            LoggerForStringLocalizerProxy = new TestLogger<StringLocalizerProxy>(testOutputHelper);

            var cultureInfoServiceMock = new Mock<ICultureInfoService>();
            var cacheServiceMock = new Mock<ICacheService>();

            this.extensionOptionsContainer = new ExtensionOptionsContainer<MyOptions>(new MyOptions());

            var optionsMock = JsonStringLocalizerFactoryTest.SetupJsonLocalizationOptionsMock(this.extensionOptionsContainer);

            // Setup extension service
            this.extensionServiceMock = new Mock<IJsonLocalizationExtensionService>();

            var extensionResolverServiceMock = JsonStringLocalizerFactoryTest.SetupResolverServiceMock(
                (this.extensionOptionsContainer, this.extensionServiceMock.Object));

            this.factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheServiceMock.Object,
                Logger,
                LoggerForStringLocalizerProxy);

            var resourceSource = this.factory.CreateStringLocalizerResourceSource(BaseName, Assembly, typeof(Component));

            this.localizer = new Mock<IStringLocalizerInternal>();
            this.localizer.SetupGet(x => x.ResourceSource).Returns(resourceSource);
        }

        [Theory]
        [InlineData(typeof(BaseClass))]
        [InlineData(typeof(Component))]
        public void ItShouldFindThroughStringLocalizerHierarchy(Type matchingType)
        {
            var localizedStringRes = this.factory.FindThroughStringLocalizerHierarchy(
                this.localizer.Object,
                CultureInfo,
                sli =>
                {
                    if (sli.ResourceSource.ResourceSourceType == matchingType)
                    {
                        return new LocalizedString("test", "test");
                    }
                    else
                    {
                        return null;
                    }
                });

            localizedStringRes.Should().NotBeNull();
            localizedStringRes!.Value.Should().Be("test");
        }

        [Fact]
        public async Task ItShouldLoadDataThroughStringLocalizerHierarchy()
        {
            await this.factory.LoadDataThroughStringLocalizerHierarchyAsync(this.localizer.Object, CultureInfo, false);

            this.localizer
                .Verify(l => l.LoadDataAsync(), Times.Once);
            this.extensionServiceMock
                .Verify(s => s.TryLoadAsync(this.extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo.Parent), Times.Once);

            var baseClassName = typeof(BaseClass).FullName ?? nameof(BaseClass);
            this.extensionServiceMock
                .Verify(s => s.TryLoadAsync(this.extensionOptionsContainer.Options, Assembly, baseClassName, CultureInfo), Times.Once);
        }
    }

    public class Component : BaseClass
    {

    }
    public class BaseClass
    {

    }
}
