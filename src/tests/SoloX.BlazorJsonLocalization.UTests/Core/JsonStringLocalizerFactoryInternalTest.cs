// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerFactoryInternalTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
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
using NSubstitute;
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

        private readonly IStringLocalizerInternal localizer;
        private readonly JsonStringLocalizerFactory factory;
        private readonly IJsonLocalizationExtensionService extensionServiceMock;
        private readonly ExtensionOptionsContainer<MyOptions> extensionOptionsContainer;

        private ILogger<JsonStringLocalizerFactory> Logger { get; }
        private ILogger<StringLocalizerProxy> LoggerForStringLocalizerProxy { get; }

        public JsonStringLocalizerFactoryInternalTest(ITestOutputHelper testOutputHelper)
        {
            Logger = new TestLogger<JsonStringLocalizerFactory>(testOutputHelper);
            LoggerForStringLocalizerProxy = new TestLogger<StringLocalizerProxy>(testOutputHelper);

            var cultureInfoServiceMock = Substitute.For<ICultureInfoService>();
            var cacheServiceMock = Substitute.For<ICacheService>();

            cacheServiceMock.Match(Arg.Any<Assembly>(), Arg.Any<string>(), Arg.Any<CultureInfo>()).Returns((IStringLocalizerInternal?)null);

            this.extensionOptionsContainer = new ExtensionOptionsContainer<MyOptions>(new MyOptions());

            var optionsMock = JsonStringLocalizerFactoryTest.SetupJsonLocalizationOptionsMock(this.extensionOptionsContainer);

            // Setup extension service
            this.extensionServiceMock = Substitute.For<IJsonLocalizationExtensionService>();

            var extensionResolverServiceMock = JsonStringLocalizerFactoryTest.SetupResolverServiceMock(
                (this.extensionOptionsContainer, this.extensionServiceMock));

            this.factory = new JsonStringLocalizerFactory(
                optionsMock,
                cultureInfoServiceMock,
                extensionResolverServiceMock,
                cacheServiceMock,
                Logger,
                LoggerForStringLocalizerProxy);

            var resourceSource = this.factory.CreateStringLocalizerResourceSource(BaseName, Assembly, typeof(Component));

            this.localizer = Substitute.For<IStringLocalizerInternal>();
            this.localizer.ResourceSource.Returns(resourceSource);
        }

        [Theory]
        [InlineData(typeof(BaseClass))]
        [InlineData(typeof(Component))]
        public void ItShouldFindThroughStringLocalizerHierarchy(Type matchingType)
        {
            var localizedStringRes = this.factory.FindThroughStringLocalizerHierarchy(
                this.localizer,
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
            await this.factory.LoadDataThroughStringLocalizerHierarchyAsync(this.localizer, CultureInfo, false);

            await this.localizer
                .Received()
                .LoadDataAsync();
            await this.extensionServiceMock
                .Received()
                .TryLoadAsync(this.extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo.Parent);

            var baseClassName = typeof(BaseClass).FullName ?? nameof(BaseClass);
            await this.extensionServiceMock
                .Received()
                .TryLoadAsync(this.extensionOptionsContainer.Options, Assembly, baseClassName, CultureInfo);
        }
    }

    public class Component : BaseClass
    {

    }
    public class BaseClass
    {

    }
}
