// ----------------------------------------------------------------------
// <copyright file="CacheServiceTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using Moq;
using SoloX.BlazorJsonLocalization.Services.Impl;
using System;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Services
{
    public class CacheServiceTest
    {
        private const string BaseName = nameof(CacheServiceTest);
        private const string CultureName = "en-US";

        private static readonly Assembly Assembly = typeof(CacheServiceTest).Assembly;

        private static readonly CultureInfo CultureInfo = CultureInfo.GetCultureInfo(CultureName);

        [Fact]
        public void ItShouldMatchARegisteredLocalizer()
        {
            var localizer = Mock.Of<IStringLocalizer>();

            var service = new CacheService();

            service.Cache(Assembly, BaseName, CultureInfo, localizer);

            var cacheEntry = service.Match(Assembly, BaseName, CultureInfo);

            Assert.NotNull(cacheEntry);
            Assert.Same(localizer, cacheEntry);
        }

        [Fact]
        public void ItShouldMatchARegisteredLocalizerNullCulture()
        {
            var localizer = Mock.Of<IStringLocalizer>();

            var service = new CacheService();

            service.Cache(Assembly, BaseName, null, localizer);

            var cacheEntry = service.Match(Assembly, BaseName, null);

            Assert.NotNull(cacheEntry);
            Assert.Same(localizer, cacheEntry);
        }

        [Fact]
        public void ItShouldResetARegisteredLocalizer()
        {
            var localizer = Mock.Of<IStringLocalizer>();

            var service = new CacheService();

            service.Cache(Assembly, BaseName, CultureInfo, localizer);

            var cacheEntry = service.Match(Assembly, BaseName, CultureInfo);

            Assert.NotNull(cacheEntry);
            Assert.Same(localizer, cacheEntry);

            Assert.True(service.Reset(Assembly, BaseName, CultureInfo));

            cacheEntry = service.Match(Assembly, BaseName, CultureInfo);

            Assert.Null(cacheEntry);
        }

        [Fact]
        public void ItShouldResetARegisteredLocalizerNullCulture()
        {
            var localizer = Mock.Of<IStringLocalizer>();

            var service = new CacheService();

            service.Cache(Assembly, BaseName, null, localizer);

            var cacheEntry = service.Match(Assembly, BaseName, null);

            Assert.NotNull(cacheEntry);
            Assert.Same(localizer, cacheEntry);

            Assert.True(service.Reset(Assembly, BaseName, null));

            cacheEntry = service.Match(Assembly, BaseName, null);

            Assert.Null(cacheEntry);
        }

        [Fact]
        public void ItShouldResetAnUnRegisteredLocalizer()
        {
            var service = new CacheService();

            Assert.False(service.Reset(Assembly, BaseName, CultureInfo));
        }

        [Fact]
        public void ItShouldResetAnUnRegisteredLocalizerNullCulture()
        {
            var service = new CacheService();

            Assert.False(service.Reset(Assembly, BaseName, null));
        }

        [Fact]
        public void ItShouldNotMatchAnUnknownLocalizer()
        {
            var service = new CacheService();

            var cacheEntry = service.Match(Assembly, BaseName, CultureInfo);

            Assert.Null(cacheEntry);
        }

        [Fact]
        public void ItShouldNotMatchAnUnknownLocalizerNullCulture()
        {
            var service = new CacheService();

            var cacheEntry = service.Match(Assembly, BaseName, null);

            Assert.Null(cacheEntry);
        }

        [Theory]
        [InlineData(typeof(CacheServiceTest), null, false)]
        [InlineData(typeof(CacheServiceTest), null, true)]
        [InlineData(null, BaseName, true)]
        public void ItShouldValidateTheArgumentsOnCache(Type type, string baseName, bool withLocalizer)
        {
            var service = new CacheService();
            var localizer = withLocalizer ? Mock.Of<IStringLocalizer>() : null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                service.Cache(
                    type?.Assembly,
                    baseName,
                    CultureInfo,
                    localizer);
            });
        }

        [Theory]
        [InlineData(typeof(CacheServiceTest), null)]
        [InlineData(null, BaseName)]
        public void ItShouldValidateTheArgumentsOnMatch(Type type, string baseName)
        {
            var service = new CacheService();

            Assert.Throws<ArgumentNullException>(() =>
            {
                service.Match(
                    type?.Assembly,
                    baseName,
                    CultureInfo);
            });
        }

        [Theory]
        [InlineData(typeof(CacheServiceTest), null)]
        [InlineData(null, BaseName)]
        public void ItShouldValidateTheArgumentsOnReset(Type type, string baseName)
        {
            var service = new CacheService();

            Assert.Throws<ArgumentNullException>(() =>
            {
                service.Reset(
                    type?.Assembly,
                    baseName,
                    CultureInfo);
            });
        }
    }
}
