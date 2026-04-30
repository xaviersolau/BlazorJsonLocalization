// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerFactoryTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.Core;
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
            RunBasicFactoryTest(f => f.Create(BaseName, Assembly.FullName!));
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
                (extensionOptionsContainer, extensionServiceMock));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var factory = new JsonStringLocalizerFactory(
                optionsMock,
                cultureInfoServiceMock,
                extensionResolverServiceMock,
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
                (extensionOptionsContainer1, extensionServiceMock1),
                (extensionOptionsContainer2, extensionServiceMock2));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer1, extensionOptionsContainer2);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var factory = new JsonStringLocalizerFactory(
                optionsMock,
                cultureInfoServiceMock,
                extensionResolverServiceMock,
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
            var extensionServiceMock = Substitute.For<IJsonLocalizationExtensionService>();
#pragma warning disable CA2012 // Use ValueTasks correctly
            extensionServiceMock
                .TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo)
                .Returns((Func<CallInfo, ValueTask<IReadOnlyDictionary<string, string>?>>)(async ci =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
                    return map;
                }));
#pragma warning restore CA2012 // Use ValueTasks correctly

            // Setup extension resolver service.
            var extensionResolverServiceMock = SetupResolverServiceMock(
                (extensionOptionsContainer, extensionServiceMock));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var factory = new JsonStringLocalizerFactory(
                optionsMock,
                cultureInfoServiceMock,
                extensionResolverServiceMock,
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
                (extensionOptionsContainer, extensionServiceMock));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer);

            var cacheServiceMock = Substitute.For<ICacheService>();

            cacheServiceMock.Match(Assembly, BaseName, Arg.Any<CultureInfo>()).Returns((IStringLocalizerInternal?)null);

            var factory = new JsonStringLocalizerFactory(
                optionsMock,
                cultureInfoServiceMock,
                extensionResolverServiceMock,
                cacheServiceMock,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(BaseName, Assembly.FullName);

            Assert.NotNull(localizer);

            cacheServiceMock.Received(1).Match(Assembly, BaseName, null);
            cacheServiceMock.Received(1).Cache(Assembly, BaseName, null, (IStringLocalizerInternal)localizer);
            cacheServiceMock.Received(1).Match(Assembly, BaseName, CultureInfo);
            cacheServiceMock.Received(1).Cache(Assembly, BaseName, CultureInfo, Arg.Any<IStringLocalizerInternal>());
        }

        [Fact]
        public void ItShouldMatchTheCacheToGetLocalizer()
        {
            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = SetupCultureInfoServiceMock();

            // Setup extension service
            var extensionServiceMock = Substitute.For<IJsonLocalizationExtensionService>();

            // Setup extension resolver service.
            var extensionResolverServiceMock = Substitute.For<IExtensionResolverService>();

            var optionsMock = JsonStringLocalizerFactoryTest.SetupJsonLocalizationOptionsMock();

            var cachedLocalizer = Substitute.For<IStringLocalizerInternal>();

            var cacheServiceMock = Substitute.For<ICacheService>();

            cacheServiceMock.Match(Assembly, BaseName, null).Returns((IStringLocalizerInternal?)null);
            cacheServiceMock.Match(Assembly, BaseName, CultureInfo).Returns(cachedLocalizer);

            var factory = new JsonStringLocalizerFactory(
                optionsMock,
                cultureInfoServiceMock,
                extensionResolverServiceMock,
                cacheServiceMock,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(BaseName, Assembly.FullName);

            Assert.NotNull(localizer);

            cacheServiceMock.Received(1).Match(Assembly, BaseName, CultureInfo);
            cacheServiceMock.Received(0).Cache(Assembly, BaseName, CultureInfo, Arg.Any<IStringLocalizerInternal>());

            var proxy = Assert.IsType<StringLocalizerProxy>(localizer);

            Assert.Same(cachedLocalizer, proxy.CurrentStringLocalizer);
        }

        [Fact]
        public void ItShouldMatchTheCacheToGetLocalizerProxy()
        {
            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = SetupCultureInfoServiceMock();

            // Setup extension service
            var extensionServiceMock = Substitute.For<IJsonLocalizationExtensionService>();

            // Setup extension resolver service.
            var extensionResolverServiceMock = Substitute.For<IExtensionResolverService>();

            var optionsMock = JsonStringLocalizerFactoryTest.SetupJsonLocalizationOptionsMock();

            var cachedLocalizer = Substitute.For<IStringLocalizer>();

            var cachedInternalLocalizer = Substitute.For<IStringLocalizerInternal>();
            cachedInternalLocalizer.AsStringLocalizer.Returns(cachedLocalizer);

            var cacheServiceMock = Substitute.For<ICacheService>();

            cacheServiceMock.Match(Assembly, BaseName, null).Returns(cachedInternalLocalizer);

            var factory = new JsonStringLocalizerFactory(
                optionsMock,
                cultureInfoServiceMock,
                extensionResolverServiceMock,
                cacheServiceMock,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(BaseName, Assembly.FullName!);

            Assert.NotNull(localizer);

            cacheServiceMock.Received(1).Match(Assembly, BaseName, null);
            cacheServiceMock.Received(0).Cache(Assembly, BaseName, null, Arg.Any<IStringLocalizerInternal>());
            cacheServiceMock.Received(0).Match(Assembly, BaseName, CultureInfo);
            cacheServiceMock.Received(0).Cache(Assembly, BaseName, CultureInfo, Arg.Any<IStringLocalizerInternal>());

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
            var extensionServiceMock = Substitute.For<IJsonLocalizationExtensionService>();

            if (isAsynchronous)
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                extensionServiceMock
                    .TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo)
                    .Returns((Func<CallInfo, ValueTask<IReadOnlyDictionary<string, string>?>>)(async (_) =>
                    {
                        await Task.Delay(100).ConfigureAwait(false);
                        return map;
                    }));
                extensionServiceMock
                    .TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo.Parent)
                    .Returns((Func<CallInfo, ValueTask<IReadOnlyDictionary<string, string>?>>)(async (_) =>
                    {
                        await Task.Delay(100).ConfigureAwait(false);
                        return map2;
                    }));
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            else
            {
                extensionServiceMock
                    .TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo)
                    .Returns(map);
                extensionServiceMock
                    .TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo.Parent)
                    .Returns(map2);
            }

            // Setup extension resolver service.
            var extensionResolverServiceMock = SetupResolverServiceMock(
                (extensionOptionsContainer, extensionServiceMock));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var factory = new JsonStringLocalizerFactory(
                optionsMock,
                cultureInfoServiceMock,
                extensionResolverServiceMock,
                cacheService,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(typeof(JsonStringLocalizerFactoryTest));

            Assert.NotNull(localizer);

            if (isAsynchronous)
            {
                // make sure the localizer data are loaded.
                await localizer.LoadAsync(true);
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
            var extensionServiceMock = Substitute.For<IJsonLocalizationExtensionService>();

#pragma warning disable CA2012 // Use ValueTasks correctly
            extensionServiceMock
                .TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo)
                .Returns((Func<CallInfo, ValueTask<IReadOnlyDictionary<string, string>?>>)(async (_) =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return map;
                }));
            extensionServiceMock
                .TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo.Parent)
                .Returns((Func<CallInfo, ValueTask<IReadOnlyDictionary<string, string>?>>)(async (_) =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return map;
                }));
#pragma warning restore CA2012 // Use ValueTasks correctly

            // Setup extension resolver service.
            var extensionResolverServiceMock = SetupResolverServiceMock(
                (extensionOptionsContainer, extensionServiceMock));

            var optionsMock = SetupJsonLocalizationOptionsMock(extensionOptionsContainer);

            // We need the real cache service because otherwise the asynchronous parent localizer will be created every time.
            var cacheService = new CacheService();

            var factory = new JsonStringLocalizerFactory(
                optionsMock,
                cultureInfoServiceMock,
                extensionResolverServiceMock,
                cacheService,
                Logger,
                LoggerForStringLocalizerProxy);

            var localizer = factory.Create(typeof(JsonStringLocalizerFactoryTest));

            Assert.NotNull(localizer);

            // make sure the localizer data are loaded.
            await localizer.LoadAsync(enableHierarchyLoading).ConfigureAwait(false);

            // verify the parent hierarchy loading behavior.
            await extensionServiceMock.Received(1).TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo);

            await extensionServiceMock
                .Received(enableHierarchyLoading ? 1 : 0)
                .TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo.Parent);
        }

        private static IJsonLocalizationExtensionService SetupExtensionServiceMock(Dictionary<string, string> map, ExtensionOptionsContainer<MyOptions> extensionOptionsContainer)
        {
            var extensionServiceMock = Substitute.For<IJsonLocalizationExtensionService>();
            extensionServiceMock
                .TryLoadAsync(extensionOptionsContainer.Options, Assembly, BaseName, CultureInfo)
                .Returns(map);
            return extensionServiceMock;
        }

        private static ICultureInfoService SetupCultureInfoServiceMock()
        {
            var cultureInfoServiceMock = Substitute.For<ICultureInfoService>();
            cultureInfoServiceMock
                .CurrentUICulture
                .Returns(CultureInfo);
            return cultureInfoServiceMock;
        }

        internal static IExtensionResolverService SetupResolverServiceMock(
            params (IExtensionOptionsContainer extensionOptionsContainer,
            IJsonLocalizationExtensionService extensionService)[] items)
        {
            var extensionResolverServiceMock = Substitute.For<IExtensionResolverService>();

            foreach (var item in items)
            {
                extensionResolverServiceMock.GetExtensionService(item.extensionOptionsContainer)
                    .Returns(item.extensionService);
            }

            return extensionResolverServiceMock;
        }

        internal static IOptions<JsonLocalizationOptions> SetupJsonLocalizationOptionsMock(params IExtensionOptionsContainer[] extensionOptionsContainers)
        {
            var opt = new JsonLocalizationOptions();
            opt.ExtensionOptions = extensionOptionsContainers;

            var optionsMock = Substitute.For<IOptions<JsonLocalizationOptions>>();

            optionsMock
                .Value
                .Returns(opt);
            return optionsMock;
        }
    }
}
