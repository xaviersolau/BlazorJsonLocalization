// ----------------------------------------------------------------------
// <copyright file="JsonLocalyserFallbackTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.ITests.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.ITests
{
    public class JsonLocalyserFallbackTest
    {
        private readonly ITestOutputHelper testOutputHelper;
        public JsonLocalyserFallbackTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("en-US", "Global", "This is global message...")]
        [InlineData("en-US", "Fallback", "This is a Fallback message...")]
        public Task ItShouldSetupEmbeddedLocalizerUsingFallbackJsonResources(string cultureName, string key, string expected)
        {
            return SetupHelper.ProcessLocalizerTestAsync<IGlobal>(
                cultureName,
                localizer =>
                {
                    Assert.NotNull(localizer);

                    var localized = localizer[key];
                    Assert.Equal(expected, localized);

                    return Task.CompletedTask;
                },
                this.testOutputHelper,
                builder => builder.AddFallback("Fallback", typeof(JsonLocalyserFallbackTest).Assembly));
        }

        [Fact]
        public async Task ItShouldLoadAsyncUsingFallback()
        {
            await SetupHelper.ProcessHttpLocalizerTestAsync<IGlobal>(
                "en-US",
                new Dictionary<string, string>
                {
                    ["Fallback.json"] = "{\"Fallback\": \"This is a fallback message...\"}",
                    ["IGlobal.json"] = "{\"Global\": \"This is global message...\"}",
                },
                async (localizer, unlocker) =>
                {
                    Assert.NotNull(localizer);

                    var globalLocalized = localizer["Global"];
                    Assert.Equal("...", globalLocalized);

                    var fallbackLocalized = localizer["Fallback"];
                    Assert.Equal("...", fallbackLocalized);

                    // Start loading async
                    var loadingTask = localizer.LoadAsync();

                    // We are sure that it is not loaded at this point (since we didn't signal any one).

                    unlocker("IGlobal.json"); // Signal Global
                    unlocker("Fallback.json"); // Signal Fallback

                    // make sure all is loaded.
                    await loadingTask.ConfigureAwait(false);

                    globalLocalized = localizer["Global"];
                    Assert.Equal("This is global message...", globalLocalized);

                    fallbackLocalized = localizer["Fallback"];
                    Assert.Equal("This is a fallback message...", fallbackLocalized);
                },
                this.testOutputHelper,
                builder => builder.AddFallback("Fallback", typeof(JsonLocalyserFallbackTest).Assembly)).ConfigureAwait(false);
        }
    }
}
