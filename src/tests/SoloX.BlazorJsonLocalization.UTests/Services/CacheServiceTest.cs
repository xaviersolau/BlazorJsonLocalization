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
        public void ItShouldNotMatchAnUnknownLocalizer()
        {
            var service = new CacheService();

            var cacheEntry = service.Match(Assembly, BaseName, CultureInfo);

            Assert.Null(cacheEntry);
        }

        [Theory]
        [InlineData(typeof(CacheServiceTest), BaseName, null, true)]
        [InlineData(typeof(CacheServiceTest), null, CultureName, true)]
        [InlineData(null, BaseName, CultureName, true)]
        [InlineData(typeof(CacheServiceTest), BaseName, CultureName, false)]
        public void ItShouldValidateTheArgumentsOnCache(Type type, string baseName, string cultureInfoName, bool withLocalizer)
        {
            var service = new CacheService();
            var localizer = withLocalizer ? Mock.Of<IStringLocalizer>() : null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                service.Cache(
                    type?.Assembly,
                    baseName,
                    cultureInfoName != null ? CultureInfo.GetCultureInfo(cultureInfoName) : null,
                    localizer);
            });
        }

        [Theory]
        [InlineData(typeof(CacheServiceTest), BaseName, null)]
        [InlineData(typeof(CacheServiceTest), null, CultureName)]
        [InlineData(null, BaseName, CultureName)]
        public void ItShouldValidateTheArgumentsOnMatch(Type type, string baseName, string cultureInfoName)
        {
            var service = new CacheService();

            Assert.Throws<ArgumentNullException>(() =>
            {
                service.Match(
                    type?.Assembly,
                    baseName,
                    cultureInfoName != null ? CultureInfo.GetCultureInfo(cultureInfoName) : null);
            });
        }
    }
}
