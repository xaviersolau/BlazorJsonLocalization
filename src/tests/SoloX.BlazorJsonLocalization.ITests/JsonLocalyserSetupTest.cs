// ----------------------------------------------------------------------
// <copyright file="JsonLocalyserSetupTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.CodeQuality.Test.Helpers.XUnit;
using System;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.ITests
{
    public class JsonLocalyserSetupTest
    {
        private readonly ITestOutputHelper testOutputHelper;
        public JsonLocalyserSetupTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("fr-FR", "Test", null, "C'est un test...")]
        [InlineData("en-US", "Test", null, "This is a test...")]
        [InlineData("fr-FR", "TestWithArg", "arg", "C'est un test avec un argument: arg...")]
        [InlineData("en-US", "TestWithArg", "arg", "This is a test with an argument: arg...")]
        public void ItShouldSetupEmbeddedLocalizer(string cultureName, string key, string arg, string expected)
        {
            ProcessSetupLocalizerTest(cultureName, localizer =>
            {
                Assert.NotNull(localizer);

                var localized = string.IsNullOrEmpty(arg)
                    ? localizer[key]
                    : localizer[key, arg];

                Assert.Equal(expected, localized.Value);
            });
        }

        [Theory]
        [InlineData("fr-FR", "Structured", "SubTest", "C'est un test structuré...")]
        [InlineData("en-US", "Structured", "SubTest", "This is a structured test...")]
        public void ItShouldSetupEmbeddedLocalizerUsingStructuredJsonResources(string cultureName, string structuredKey, string key, string expected)
        {
            ProcessSetupLocalizerTest(cultureName, localizer =>
            {
                Assert.NotNull(localizer);

                var localized = localizer[structuredKey + Key.Separator + key];
                Assert.Equal(expected, localized);

                var localizedWithGet = localizer[Key.Path(structuredKey, key)];
                Assert.Equal(expected, localized);

                var subLocalizer = localizer.GetSubLocalizer(structuredKey);

                var subLocalized = subLocalizer[key];

                Assert.Equal(expected, subLocalized.Value);
            });
        }

        private void ProcessSetupLocalizerTest(string cultureName, Action<IStringLocalizer<JsonLocalyserSetupTest>> testHandler)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var cultureInfoServiceMock = new Mock<ICultureInfoService>();

            cultureInfoServiceMock.SetupGet(s => s.CurrentUICulture).Returns(cultureInfo);

            var services = new ServiceCollection();
            services.AddTestLogging(this.testOutputHelper);

            services.AddJsonLocalization(
                builder => builder.UseEmbeddedJson(options => options.ResourcesPath = "Resources"));

            services.AddSingleton(cultureInfoServiceMock.Object);
            using var provider = services.BuildServiceProvider();
            var localizer = provider.GetRequiredService<IStringLocalizer<JsonLocalyserSetupTest>>();

            testHandler(localizer);
        }
    }
}
