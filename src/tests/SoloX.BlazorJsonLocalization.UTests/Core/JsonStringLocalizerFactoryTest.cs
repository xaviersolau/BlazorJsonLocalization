// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerFactoryTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Moq;
using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.UTests.Samples.Extension;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class JsonStringLocalizerFactoryTest
    {
        private const string CultureName = "en-us";

        private static readonly string BaseName = typeof(JsonStringLocalizerFactoryTest).FullName ?? nameof(JsonStringLocalizerFactoryTest);
        private static readonly Assembly Assembly = typeof(JsonStringLocalizerFactoryTest).Assembly;
        private static readonly CultureInfo CultureInfo = CultureInfo.GetCultureInfo(CultureName);

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

        private static void RunBasicFactoryTest(Func<JsonStringLocalizerFactory, IStringLocalizer> getLocalizer)
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
                cacheServiceMock.Object);

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
                cacheServiceMock.Object);

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
                cacheServiceMock.Object);

            var localizer = factory.Create(typeof(JsonStringLocalizerFactoryTest));

            Assert.NotNull(localizer);

            Assert.IsType<JsonStringLocalizerAsync>(localizer);
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
                cacheServiceMock.Object);

            var localizer = factory.Create(BaseName, Assembly.FullName);

            Assert.NotNull(localizer);

            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, CultureInfo), Times.Once);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, CultureInfo, localizer), Times.Once);
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
                cacheServiceMock.Object);

            var localizer = factory.Create(BaseName, Assembly.FullName);

            Assert.NotNull(localizer);

            cacheServiceMock.Verify(x => x.Match(Assembly, BaseName, CultureInfo), Times.Once);
            cacheServiceMock.Verify(x => x.Cache(Assembly, BaseName, CultureInfo, It.IsAny<IStringLocalizer>()), Times.Never);

            Assert.Same(cachedLocalizer, localizer);
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
