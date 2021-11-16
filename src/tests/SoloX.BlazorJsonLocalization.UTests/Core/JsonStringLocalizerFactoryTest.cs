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

        public JsonStringLocalizerFactoryTest(ITestOutputHelper testOutputHelper)
        {
            Logger = new TestLogger<JsonStringLocalizerFactory>(testOutputHelper);
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

            var cacheServiceMock = new Mock<ICacheService>();

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheServiceMock.Object,
                Logger);

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
            extensionOptionsContainer1.Options.AssemblyNames = new[] { "OtherAssemblyName" };
            var extensionOptionsContainer2 = new ExtensionOptionsContainer<MyOptions>(new MyOptions());
            extensionOptionsContainer2.Options.AssemblyNames = new[] { Assembly.GetName().Name };

            // Setup extension service
            var extensionServiceMock1 = SetupExtensionServiceMock(map1, extensionOptionsContainer1);
            var extensionServiceMock2 = SetupExtensionServiceMock(map2, extensionOptionsContainer2);

            // Setup extension resolver service.
            var extensionResolverServiceMock = SetupResolverServiceMock(
                (extensionOptionsContainer1, extensionServiceMock1.Object),
                (extensionOptionsContainer2, extensionServiceMock2.Object));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer1, extensionOptionsContainer2);

            var cacheServiceMock = new Mock<ICacheService>();

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheServiceMock.Object,
                Logger);

            var localizer = factory.Create(typeof(JsonStringLocalizerFactoryTest));

            Assert.NotNull(localizer);

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

            var cacheServiceMock = new Mock<ICacheService>();

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheServiceMock.Object,
                Logger);

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

            var cacheServiceMock = new Mock<ICacheService>();

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheServiceMock.Object,
                Logger);

            var localizer = factory.Create(BaseName, Assembly.FullName);

            Assert.NotNull(localizer);

            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, null), Times.Once);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, null, localizer), Times.Once);
            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, CultureInfo), Times.Once);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, CultureInfo, It.IsAny<IStringLocalizer>()), Times.Once);
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

            var optionsMock = new Mock<IOptions<JsonLocalizationOptions>>();

            var cacheServiceMock = new Mock<ICacheService>();

            var cachedLocalizer = Mock.Of<IStringLocalizer>();

            cacheServiceMock.Setup(x => x.Match(Assembly, BaseName, CultureInfo)).Returns(cachedLocalizer);

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheServiceMock.Object,
                Logger);

            var localizer = factory.Create(BaseName, Assembly.FullName);

            Assert.NotNull(localizer);

            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, CultureInfo), Times.Once);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, CultureInfo, It.IsAny<IStringLocalizer>()), Times.Never);

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

            var optionsMock = new Mock<IOptions<JsonLocalizationOptions>>();

            var cacheServiceMock = new Mock<ICacheService>();

            var cachedLocalizer = Mock.Of<IStringLocalizer>();

            cacheServiceMock.Setup(x => x.Match(Assembly, BaseName, null)).Returns(cachedLocalizer);

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object,
                cacheServiceMock.Object,
                Logger);

            var localizer = factory.Create(BaseName, Assembly.FullName);

            Assert.NotNull(localizer);

            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, null), Times.Once);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, null, It.IsAny<IStringLocalizer>()), Times.Never);
            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, CultureInfo), Times.Never);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, CultureInfo, It.IsAny<IStringLocalizer>()), Times.Never);

            Assert.Same(cachedLocalizer, localizer);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ItShouldUseParentCultureLocalyzerWhenKeyNotFound(bool isAsynchronous)
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

            var unknownKey = "test";

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
                Logger);

            var localizer = factory.Create(typeof(JsonStringLocalizerFactoryTest));

            if (isAsynchronous)
            {
                // make sure the localizer data are loaded.
                await localizer.LoadAsync(true).ConfigureAwait(false);
            }

            Assert.NotNull(localizer);

            Assert.Equal(value, localizer[key]);
            Assert.Equal(value2, localizer[key2]);

            Assert.Equal(unknownKey, localizer[unknownKey]);

            var strings = localizer.GetAllStrings(false);
            var allStrings = localizer.GetAllStrings(true);

            Assert.Single(strings);
            Assert.Equal(2, allStrings.Count());
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

        private static Mock<IExtensionResolverService> SetupResolverServiceMock(
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

        private static Mock<IOptions<JsonLocalizationOptions>> SetupJsonLocalizationOptionsMock(params IExtensionOptionsContainer[] extensionOptionsContainers)
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
