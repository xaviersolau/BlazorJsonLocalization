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
using Moq;
using Xunit;
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Core;

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
                await Task.Yield();

                // Wait until signal.
                waitHandler.WaitOne();

                // Return data
                return new Dictionary<string, string>()
                {
                    [key] = txt
                };
            };

            var localizerFactoryInternalMock = new Mock<IJsonStringLocalizerFactoryInternal>();

            var loadingLocalizerMock = new Mock<IStringLocalizer>();
            loadingLocalizerMock.SetupGet(x => x[key]).Returns(new LocalizedString(key, "..."));

            var loadingLocalizerInternalMock = new Mock<IStringLocalizerInternal>();
            loadingLocalizerInternalMock.SetupGet(x => x.AsStringLocalizer).Returns(loadingLocalizerMock.Object);

            localizerFactoryInternalMock.Setup(x => x.CreateStringLocalizer(It.IsAny<CultureInfo>())).Returns(loadingLocalizerInternalMock.Object);


            // Setup the async localizer with the loadAsync task.
            var localizer = new JsonStringLocalizerAsync(
                loadAsync(),
                cultureInfo,
                localizerFactoryInternalMock.Object,
                loadingLocalizerInternalMock.Object);

            // Make sure the localized text is loading...
            var loading = localizer[key];
            Assert.Equal("...", loading.Value);

            // Signal the async task.
            waitHandler.Set();

            // Make sure the localized text is loaded.
            await JsonStringLocalizerAsync.LoadAsync(localizer, false).ConfigureAwait(false);

            // Make sure the localized text is the one expected.
            var value = localizer[key];
            Assert.Equal(txt, value.Value);

        }
    }
}
