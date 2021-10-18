// ----------------------------------------------------------------------
// <copyright file="EmbeddedJsonLocalizationExtensionUsingAccentsTest.cs" company="Xavier Solau">
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
    public class EmbeddedJsonLocalizationExtensionUsingAccentsTest
    {
        private const string BaseName = nameof(EmbeddedJsonLocalizationExtensionUsingAccentsTest);

        private static readonly Assembly Assembly = typeof(EmbeddedJsonLocalizationExtensionUsingAccentsTest).Assembly;

        private ILogger<EmbeddedJsonLocalizationExtensionService> Logger { get; }

        public EmbeddedJsonLocalizationExtensionUsingAccentsTest(ITestOutputHelper testOutputHelper)
        {
            Logger = new TestLogger<EmbeddedJsonLocalizationExtensionService>(testOutputHelper);
        }

        [Fact]
        public async Task ItShouldLoadTheAppropriateMapAsync()
        {
            var cultureName = "en-US";
            var resourcePath = "Resources";
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            var service = new EmbeddedJsonLocalizationExtensionService(Logger);

            var options = new EmbeddedJsonLocalizationOptions();
            options.ResourcesPath = resourcePath;

            var map = await service.TryLoadAsync(options, Assembly, BaseName, cultureInfo).ConfigureAwait(false);

            Assert.NotNull(map);
            Assert.Equal("Text for auswählen", map["auswählen"]);
            Assert.Equal("Text for été", map["été"]);
            Assert.Equal("こんにちは", map["konnichiwa"]);
            Assert.Equal("سعادة", map["Happiness"]);
        }
    }
}
