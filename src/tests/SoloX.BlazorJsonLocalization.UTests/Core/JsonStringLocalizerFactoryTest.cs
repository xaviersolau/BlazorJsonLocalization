// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerFactoryTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.Services.Impl;
using SoloX.BlazorJsonLocalization.UTests.Samples.Extension;
using SoloX.CodeQuality.Test.Helpers.XUnit.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class JsonStringLocalizerFactoryTest
    {
        private const string CultureName = "en-us";

        private static readonly string BaseName = typeof(JsonStringLocalizerFactoryTest).FullName ?? nameof(JsonStringLocalizerFactoryTest);
        private static readonly Assembly Assembly = typeof(JsonStringLocalizerFactoryTest).Assembly;
        private static readonly CultureInfo CultureInfo = CultureInfo.GetCultureInfo(CultureName);

        private ILogger<JsonStringLocalizerFactory> Logger { get; }
        private ILogger<StringLocalizerProxy> LoggerForStringLocalizerProxy { get; }

        public JsonStringLocalizerFactoryTest(ITestOutputHelper testOutputHelper)
        {
            Logger = new TestLogger<JsonStringLocalizerFactory>(testOutputHelper);
            LoggerForStringLocalizerProxy = new TestLogger<StringLocalizerProxy>(testOutputHelper);
        }

        [Fact]
        public void IsShouldCreateAJsonStringLocalizerMatchingTheGivenBaseNameAndLocation()
        {
            RunBasicFactoryTest(f => f.Create(BaseName, Assembly.FullName));
        }

        [Fact]
        public void IsShouldCreateAJsonStringLocalizerMatchingTheGivenType()
        {
            RunBasicFactoryTest(f => f.Create(typeof(JsonStringLocalizerFactoryTest)));
        }

        private void RunBasicFactoryTest(Func<JsonStringLocalizerFactory, IStringLocalizer> getLocalizer)
        {
            var key = "key";
            var value = "value";
            var map = new Dictionary<string, string>
            {
                [key] = value,
            };

            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = SetupCultureInfoServiceMock();

            var extensionOptionsContainer = new ExtensionOptionsContainer<MyOptions>(new MyOptions());

            // Setup extension service
            var extensionServiceMock = SetupExtensionServiceMock(map, extensionOptionsContainer);

            // Setup extension resolver service.
            var extensionResolverServiceMock = SetupResolverServiceMock(
                (extensionOptionsContainer, extensionServiceMock.Object));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheService,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = getLocalizer(factory);

            Assert.NotNull(localizer);

            Assert.Equal(value, localizer[key]);
        }

        [Fact]
        public void IsShouldCreateAJsonStringLocalizerUsingTheRightExtensionService()
        {
            var key = "key";
            var value1 = "value1";
            var map1 = new Dictionary<string, string>
            {
                [key] = value1,
            };
            var value2 = "value2";
            var map2 = new Dictionary<string, string>
            {
                [key] = value2,
            };

            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = SetupCultureInfoServiceMock();

            var extensionOptionsContainer1 = new ExtensionOptionsContainer<MyOptions>(new MyOptions());
            extensionOptionsContainer1.Options.Assemblies = new[] { typeof(Type).Assembly }; // Use another assembly
            var extensionOptionsContainer2 = new ExtensionOptionsContainer<MyOptions>(new MyOptions());
            extensionOptionsContainer2.Options.Assemblies = new[] { Assembly };

            // Setup extension service
            var extensionServiceMock1 = SetupExtensionServiceMock(map1, extensionOptionsContainer1);
            var extensionServiceMock2 = SetupExtensionServiceMock(map2, extensionOptionsContainer2);

            // Setup extension resolver service.
            var extensionResolverServiceMock = SetupResolverServiceMock(
                (extensionOptionsContainer1, extensionServiceMock1.Object),
                (extensionOptionsContainer2, extensionServiceMock2.Object));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer1, extensionOptionsContainer2);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheService,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(typeof(JsonStringLocalizerFactoryTest));

            Assert.NotNull(localizer);

            // Make sure the selected localizer is the one resulting from extensionOptionsContainer2.
            Assert.Equal(value2, localizer[key]);
        }

        [Fact]
        public void ItShouldCreateAnAsynchronousLocalizerIfTheExtensionServiceIsAsynchronous()
        {
            var key = "key";
            var value = "value";
            var map = new Dictionary<string, string>
            {
                [key] = value,
            };

            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = SetupCultureInfoServiceMock();

            var extensionOptionsContainer = new ExtensionOptionsContainer<MyOptions>(new MyOptions());

            // Setup extension service
            var extensionServiceMock = new Mock<IJsonLocalizationExtensionService>();
            extensionServiceMock
                .Setup(s => s.TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo))
                .ReturnsAsync(map, TimeSpan.FromMilliseconds(100));

            // Setup extension resolver service.
            var extensionResolverServiceMock = SetupResolverServiceMock(
                (extensionOptionsContainer, extensionServiceMock.Object));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheService,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(typeof(JsonStringLocalizerFactoryTest));

            Assert.NotNull(localizer);

            var proxy = Assert.IsType<StringLocalizerProxy>(localizer);

            Assert.IsType<JsonStringLocalizerAsync>(proxy.CurrentStringLocalizer);
        }

        [Fact]
        public void ItShouldCacheTheComputedLocalizer()
        {
            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = SetupCultureInfoServiceMock();

            var extensionOptionsContainer = new ExtensionOptionsContainer<MyOptions>(new MyOptions());

            // Setup extension service
            var extensionServiceMock = SetupExtensionServiceMock(new Dictionary<string, string>(), extensionOptionsContainer);

            // Setup extension resolver service.
            var extensionResolverServiceMock = SetupResolverServiceMock(
                (extensionOptionsContainer, extensionServiceMock.Object));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheServiceMock = new Mock<ICacheService>();

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheServiceMock.Object,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(BaseName, Assembly.FullName);

            Assert.NotNull(localizer);

            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, null), Times.Once);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, null, (IStringLocalizerInternal)localizer), Times.Once);
            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, CultureInfo), Times.Once);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, CultureInfo, It.IsAny<IStringLocalizerInternal>()), Times.Once);
        }

        [Fact]
        public void ItShouldMatchTheCacheToGetLocalizer()
        {
            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = SetupCultureInfoServiceMock();

            // Setup extension service
            var extensionServiceMock = new Mock<IJsonLocalizationExtensionService>();

            // Setup extension resolver service.
            var extensionResolverServiceMock = new Mock<IExtensionResolverService>();

            var optionsMock = JsonStringLocalizerFactoryTest.SetupJsonLocalizationOptionsMock();

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var cachedLocalizer = Mock.Of<IStringLocalizerInternal>();

            var cacheServiceMock = new Mock<ICacheService>();

            cacheServiceMock.Setup(x => x.Match(Assembly, BaseName, CultureInfo)).Returns(cachedLocalizer);

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheServiceMock.Object,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(BaseName, Assembly.FullName);

            Assert.NotNull(localizer);

            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, CultureInfo), Times.Once);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, CultureInfo, It.IsAny<IStringLocalizerInternal>()), Times.Never);

            var proxy = Assert.IsType<StringLocalizerProxy>(localizer);

            Assert.Same(cachedLocalizer, proxy.CurrentStringLocalizer);
        }

        [Fact]
        public void ItShouldMatchTheCacheToGetLocalizerProxy()
        {
            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = SetupCultureInfoServiceMock();

            // Setup extension service
            var extensionServiceMock = new Mock<IJsonLocalizationExtensionService>();

            // Setup extension resolver service.
            var extensionResolverServiceMock = new Mock<IExtensionResolverService>();

            var optionsMock = JsonStringLocalizerFactoryTest.SetupJsonLocalizationOptionsMock();

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var cachedLocalizer = Mock.Of<IStringLocalizer>();

            var cachedInternalLocalizer = new Mock<IStringLocalizerInternal>();
            cachedInternalLocalizer.SetupGet(x => x.AsStringLocalizer).Returns(cachedLocalizer);

            var cacheServiceMock = new Mock<ICacheService>();

            cacheServiceMock.Setup(x => x.Match(Assembly, BaseName, null)).Returns(cachedInternalLocalizer.Object);

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheServiceMock.Object,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(BaseName, Assembly.FullName);

            Assert.NotNull(localizer);

            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, null), Times.Once);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, null, It.IsAny<IStringLocalizerInternal>()), Times.Never);
            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, CultureInfo), Times.Never);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, CultureInfo, It.IsAny<IStringLocalizerInternal>()), Times.Never);

            Assert.Same(cachedLocalizer, localizer);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ItShouldUseParentCultureLocalizerWhenKeyNotFound(bool isAsynchronous)
        {
            var key = "key";
            var value = "value";
            var map = new Dictionary<string, string>
            {
                [key] = value,
            };

            var key2 = "key2";
            var value2 = "value2";
            var map2 = new Dictionary<string, string>
            {
                [key2] = value2,
            };

            var unknownKey = "unknownKey";

            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = SetupCultureInfoServiceMock();

            var extensionOptionsContainer = new ExtensionOptionsContainer<MyOptions>(new MyOptions());

            // Setup extension service
            var extensionServiceMock = new Mock<IJsonLocalizationExtensionService>();

            if (isAsynchronous)
            {
                extensionServiceMock
                    .Setup(s => s.TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo))
                    .ReturnsAsync(map, TimeSpan.FromMilliseconds(100));
                extensionServiceMock
                    .Setup(s => s.TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo.Parent))
                    .ReturnsAsync(map2, TimeSpan.FromMilliseconds(100));
            }
            else
            {
                extensionServiceMock
                    .Setup(s => s.TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo))
                    .ReturnsAsync(map);
                extensionServiceMock
                    .Setup(s => s.TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo.Parent))
                    .ReturnsAsync(map2);
            }

            // Setup extension resolver service.
            var extensionResolverServiceMock = SetupResolverServiceMock(
                (extensionOptionsContainer, extensionServiceMock.Object));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheService,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(typeof(JsonStringLocalizerFactoryTest));

            Assert.NotNull(localizer);

            if (isAsynchronous)
            {
                // make sure the localizer data are loaded.
                await localizer.LoadAsync(true).ConfigureAwait(false);
            }

            Assert.Equal(value, localizer[key]);
            Assert.Equal(value2, localizer[key2]);

            var unknownStringLocalizer = localizer[unknownKey];
            Assert.True(unknownStringLocalizer.ResourceNotFound);

            var strings = localizer.GetAllStrings(false);
            var allStrings = localizer.GetAllStrings(true);

            Assert.Single(strings);
            Assert.Equal(2, allStrings.Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ItShouldLoadParentCultureLocalyzer(bool enableHierarchyLoading)
        {
            var map = new Dictionary<string, string>();

            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = SetupCultureInfoServiceMock();

            var extensionOptionsContainer = new ExtensionOptionsContainer<MyOptions>(new MyOptions());

            // Setup extension service
            var extensionServiceMock = new Mock<IJsonLocalizationExtensionService>();

            extensionServiceMock
                .Setup(s => s.TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo))
                .ReturnsAsync(map, TimeSpan.FromMilliseconds(100));
            extensionServiceMock
                .Setup(s => s.TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo.Parent))
                .ReturnsAsync(map, TimeSpan.FromMilliseconds(100));

            // Setup extension resolver service.
            var extensionResolverServiceMock = SetupResolverServiceMock(
                (extensionOptionsContainer, extensionServiceMock.Object));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheService,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(typeof(JsonStringLocalizerFactoryTest));

            Assert.NotNull(localizer);

            // make sure the localizer data are loaded.
            await localizer.LoadAsync(enableHierarchyLoading).ConfigureAwait(false);

            // verify the parent hierarchy loading behavior.
            extensionServiceMock.Verify(s => s.TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo), Times.Once);

            extensionServiceMock.Verify(s => s.TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo.Parent), enableHierarchyLoading ? Times.Once : Times.Never);
        }

        private static Mock<IJsonLocalizationExtensionService> SetupExtensionServiceMock(Dictionary<string, string> map, ExtensionOptionsContainer<MyOptions> extensionOptionsContainer)
        {
            var extensionServiceMock = new Mock<IJsonLocalizationExtensionService>();
            extensionServiceMock
                .Setup(s => s.TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo))
                .ReturnsAsync(map);
            return extensionServiceMock;
        }

        private static Mock<ICultureInfoService> SetupCultureInfoServiceMock()
        {
            var cultureInfoServiceMock = new Mock<ICultureInfoService>();
            cultureInfoServiceMock
                .SetupGet(s => s.CurrentUICulture)
                .Returns(CultureInfo);
            return cultureInfoServiceMock;
        }

        internal static Mock<IExtensionResolverService> SetupResolverServiceMock(
            params (IExtensionOptionsContainer extensionOptionsContainer,
            IJsonLocalizationExtensionService extensionService)[] items)
        {
            var extensionResolverServiceMock = new Mock<IExtensionResolverService>();

            foreach (var item in items)
            {
                extensionResolverServiceMock.Setup(s => s.GetExtensionService(item.extensionOptionsContainer))
                    .Returns(item.extensionService);
            }

            return extensionResolverServiceMock;
        }

        internal static Mock<IOptions<JsonLocalizationOptions>> SetupJsonLocalizationOptionsMock(params IExtensionOptionsContainer[] extensionOptionsContainers)
        {
            var opt = new JsonLocalizationOptions();
            opt.ExtensionOptions = extensionOptionsContainers;

            var optionsMock = new Mock<IOptions<JsonLocalizationOptions>>();

            optionsMock
                .SetupGet(o => o.Value)
                .Returns(opt);
            return optionsMock;
        }
    }
}
