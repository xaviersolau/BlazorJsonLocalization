// ----------------------------------------------------------------------
// <copyright file="JsonLocalyserWebAssemblySetupTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using SoloX.BlazorJsonLocalization.ITests.Utils;
using System.Collections.Generic;

namespace SoloX.BlazorJsonLocalization.ITests
{
    public class JsonLocalyserWebAssemblySetupTest
    {
        private readonly ITestOutputHelper testOutputHelper;
        public JsonLocalyserWebAssemblySetupTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("fr-FR", "Test", null, "C'est un test...")]
        [InlineData("en-US", "Test", null, "This is a test...")]
        [InlineData("fr-FR", "TestWithArg", "arg", "C'est un test avec un argument: arg...")]
        [InlineData("en-US", "TestWithArg", "arg", "This is a test with an argument: arg...")]
        public async Task ItShouldSetupWebAssemblyHttpHostedLocalizerAsync(string cultureName, string key, string arg, string expected)
        {
            await SetupHelper.ProcessHttpLocalizerTestAsync<JsonLocalyserWebAssemblySetupTest>(
                cultureName,
                new Dictionary<string, string>
                {
                    ["JsonLocalyserWebAssemblySetupTest.json"] = "{\"Test\": \"This is a test...\", \"TestWithArg\": \"This is a test with an argument: {0}...\"}",
                    ["JsonLocalyserWebAssemblySetupTest.fr.json"] = "{\"Test\": \"C'est un test...\", \"TestWithArg\": \"C'est un test avec un argument: {0}...\"}",
                },
                async localizer =>
                {
                    Assert.NotNull(localizer);

                    await localizer.LoadAsync().ConfigureAwait(false);

                    var localized = string.IsNullOrEmpty(arg)
                        ? localizer[key]
                        : localizer[key, arg];

                    Assert.Equal(expected, localized.Value);
                },
                this.testOutputHelper).ConfigureAwait(false);
        }
    }
}
