// ----------------------------------------------------------------------
// <copyright file="EmbeddedJsonLocalizationExtensionServiceTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Services.Impl;
using SoloX.CodeQuality.Test.Helpers.XUnit.Logger;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.UTests.Services
{
    public class EmbeddedJsonLocalizationExtensionServiceTest
    {
        private const string BaseName = nameof(EmbeddedJsonLocalizationExtensionServiceTest);

        private static readonly Assembly Assembly = typeof(EmbeddedJsonLocalizationExtensionServiceTest).Assembly;

        private ILogger<EmbeddedJsonLocalizationExtensionService> Logger { get; }

        public EmbeddedJsonLocalizationExtensionServiceTest(ITestOutputHelper testOutputHelper)
        {
            Logger = new TestLogger<EmbeddedJsonLocalizationExtensionService>(testOutputHelper);
        }

        [Theory]
        [InlineData("en-US", "Test", "English test.", true, "Resources")]
        [InlineData("fr-FR", "Test", "French test.", true, "Resources")]
        [InlineData("en-US", "Test", null, false, "Bad")]
        [InlineData("fr-FR", "Test", null, false, "Bad")]
        [InlineData("en-US", "Test", null, false, null)]
        [InlineData("fr-FR", "Test", null, false, null)]
        public async Task ItShouldLoadTheAppropriateMapAsync(
            string cultureName,
            string key,
            string expectedText,
            bool expectedSuccess,
            string resourcePath)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            var service = new EmbeddedJsonLocalizationExtensionService(Logger);

            var options = new EmbeddedJsonLocalizationOptions();
            options.ResourcesPath = resourcePath;

            var map = await service.TryLoadAsync(options, Assembly, BaseName, cultureInfo).ConfigureAwait(false);

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

        [Theory]
        [InlineData("en-US", "Structured:Test", "English structured test.", true, "Resources")]
        [InlineData("fr-FR", "Structured:Test", "French structured test.", true, "Resources")]
        public async Task ItShouldLoadTheAppropriateMapFromAStructuredJsonFileAsync(
            string cultureName,
            string key,
            string expectedText,
            bool expectedSuccess,
            string resourcePath)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            var service = new EmbeddedJsonLocalizationExtensionService(Logger);

            var options = new EmbeddedJsonLocalizationOptions();
            options.ResourcesPath = resourcePath;

            var map = await service.TryLoadAsync(options, Assembly, BaseName, cultureInfo).ConfigureAwait(false);

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
