// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerAsyncTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core.Impl;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class JsonStringLocalizerAsyncTest
    {
        [Fact]
        public async Task ItShoultLoadLocalizerDataAsync()
        {
            var key = "TestKey";
            var txt = "Test txt value.";

            var cultureInfo = CultureInfo.GetCultureInfo("fr-FR");
            using var waitHandler = new ManualResetEvent(false);

            async Task<IReadOnlyDictionary<string, string>> loadAsync()
            {
                // Make sure the task is running asynchronously.
                await Task.Delay(1).ConfigureAwait(false);

                // Wait until signal.
                waitHandler.WaitOne();

                // Return data
                return new Dictionary<string, string>()
                {
                    [key] = txt
                };
            };

            // Setup the async localizer with the loadAsync task.
            var localizer = new JsonStringLocalizerAsync(
                loadAsync(),
                cultureInfo);

            // Make sure the localized text is loading...
            var loading = localizer[key];
            Assert.Equal("...", loading.Value);

            // Signal the async task.
            waitHandler.Set();

            // Make the localized text is loaded.
            await JsonStringLocalizerAsync.LoadAsync(localizer).ConfigureAwait(false);

            // Make sure the localized text is the one expected.
            var value = localizer[key];
            Assert.Equal(txt, value.Value);

        }
    }
}
