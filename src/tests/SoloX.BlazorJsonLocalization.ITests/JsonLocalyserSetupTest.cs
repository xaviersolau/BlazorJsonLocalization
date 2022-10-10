// ----------------------------------------------------------------------
// <copyright file="JsonLocalyserSetupTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.ITests.Utils;
using System;
using System.Threading.Tasks;
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
        public Task ItShouldSetupEmbeddedLocalizer(string cultureName, string key, string arg, string expected)
        {
            return ProcessSetupLocalizerTest(cultureName, localizer =>
            {
                Assert.NotNull(localizer);

                var localized = string.IsNullOrEmpty(arg)
                    ? localizer[key]
                    : localizer[key, arg];

                Assert.Equal(expected, localized.Value);

                return Task.CompletedTask;
            });
        }

        [Theory]
        [InlineData("fr-FR", "Structured", "SubTest", "C'est un test structuré...")]
        [InlineData("en-US", "Structured", "SubTest", "This is a structured test...")]
        public Task ItShouldSetupEmbeddedLocalizerUsingStructuredJsonResources(string cultureName, string structuredKey, string key, string expected)
        {
            return ProcessSetupLocalizerTest(cultureName, localizer =>
            {
                Assert.NotNull(localizer);

                var localized = localizer[structuredKey + Key.Separator + key];
                Assert.Equal(expected, localized);

                var localizedWithGet = localizer[Key.Path(structuredKey, key)];
                Assert.Equal(expected, localized);

                var subLocalizer = localizer.GetSubLocalizer(structuredKey);

                var subLocalized = subLocalizer[key];

                Assert.Equal(expected, subLocalized.Value);

                return Task.CompletedTask;
            });
        }

        private Task ProcessSetupLocalizerTest(string cultureName, Func<IStringLocalizer<JsonLocalyserSetupTest>, Task> testHandler)
        {
            return SetupHelper.ProcessLocalizerTestAsync(cultureName, testHandler, this.testOutputHelper);
        }
    }
}
