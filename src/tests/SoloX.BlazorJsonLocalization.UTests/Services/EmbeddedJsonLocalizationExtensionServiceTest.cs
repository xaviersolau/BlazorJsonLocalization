// ----------------------------------------------------------------------
// <copyright file="EmbeddedJsonLocalizationExtensionServiceTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Services.Impl;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Services
{
    public class EmbeddedJsonLocalizationExtensionServiceTest
    {
        private const string BaseName = nameof(EmbeddedJsonLocalizationExtensionServiceTest);

        private static readonly Assembly Assembly = typeof(EmbeddedJsonLocalizationExtensionServiceTest).Assembly;

        [Theory]
        [InlineData("en-us", "Test", "English test.", true, "Resources")]
        [InlineData("fr-fr", "Test", "French test.", true, "Resources")]
        [InlineData("en-us", "Test", null, false, "Bad")]
        [InlineData("fr-fr", "Test", null, false, "Bad")]
        [InlineData("en-us", "Test", null, false, null)]
        [InlineData("fr-fr", "Test", null, false, null)]
        public void ItShouldLoadTheAppropriateMap(
            string cultureName,
            string key,
            string expectedText,
            bool expectedSuccess,
            string resourcePath)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            var service = new EmbeddedJsonLocalizationExtensionService();

            var options = new EmbeddedJsonLocalizationOptions();
            options.ResourcesPath = resourcePath;

            var map = service.TryLoad(options, Assembly, BaseName, cultureInfo);

            if (expectedSuccess)
            {
                Assert.NotNull(map);
                Assert.Equal(expectedText, map[key]);
            }
            else
            {
                Assert.Null(map);
            }
        }
    }
}
