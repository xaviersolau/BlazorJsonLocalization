// ----------------------------------------------------------------------
// <copyright file="JsonLocalyserInheritTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.ITests.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.ITests
{
    public class JsonLocalyserInheritTest
    {
        private readonly ITestOutputHelper testOutputHelper;
        public JsonLocalyserInheritTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("en-US", "Global", "This is global message...")]
        [InlineData("en-US", "Specific", "This is specific message...")]
        public Task ItShouldSetupEmbeddedLocalizerUsingInheritedJsonResources(string cultureName, string key, string expected)
        {
            return SetupHelper.ProcessLocalizerTestAsync<ISpecific>(
                cultureName,
                localizer =>
                {
                    Assert.NotNull(localizer);

                    var localized = localizer[key];
                    Assert.Equal(expected, localized);

                    return Task.CompletedTask;
                },
                this.testOutputHelper);
        }

        [Fact]
        public async Task ItShouldLoadAsyncThroughHierarchy()
        {
            using var waitHandler = new AutoResetEvent(false);

            await SetupHelper.ProcessHttpLocalizerTestAsync<ISpecific>(
                "en-US",
                new Dictionary<string, string>
                {
                    ["ISpecific.json"] = "{\"Specific\": \"This is specific message...\"}",
                    ["IGlobal.json"] = "{\"Global\": \"This is global message...\"}",
                },
                async localizer =>
                {
                    Assert.NotNull(localizer);

                    var specificLocalized = localizer["Specific"];
                    Assert.Equal("...", specificLocalized);

                    var globalLocalized = localizer["Global"];
                    Assert.Equal("...", globalLocalized);

                    // Start loading async
                    var loadingTask = localizer.LoadAsync();

                    // We are sure that it is not loaded at this point (since we didn't signal any one).

                    waitHandler.Set(); // Signal Specific

                    // Make sure Specific string is available.
                    var nbRetry = 5;
                    for (var i = 0; i <= nbRetry; i++)
                    {
                        await Task.Delay(100).ConfigureAwait(false);

                        specificLocalized = localizer["Specific"];
                        if ("This is specific message..." == specificLocalized)
                        {
                            break;
                        }
                        else
                        {
                            Assert.True(i < nbRetry);
                        }
                    }

                    // Global must not be loaded.
                    globalLocalized = localizer["Global"];
                    Assert.Equal("...", globalLocalized);

                    // Loading task must not be completed.
                    Assert.False(loadingTask.IsCompleted);

                    waitHandler.Set(); // Signal Global

                    // make sure all is loaded.
                    await loadingTask.ConfigureAwait(false);

                    specificLocalized = localizer["Specific"];
                    Assert.Equal("This is specific message...", specificLocalized);

                    globalLocalized = localizer["Global"];
                    Assert.Equal("This is global message...", globalLocalized);
                },
                this.testOutputHelper,
                waitHandler).ConfigureAwait(false);
        }
    }
}
