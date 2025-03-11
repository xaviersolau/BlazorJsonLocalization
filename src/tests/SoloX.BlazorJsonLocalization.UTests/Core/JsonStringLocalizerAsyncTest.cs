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
using System.Threading.Tasks;
using Moq;
using Xunit;
using SoloX.BlazorJsonLocalization.Core;
using Microsoft.Extensions.Localization;
using System;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class JsonStringLocalizerAsyncTest
    {
        [Theory]
        [InlineData("TestKey", "TestText", "TestKey", null, "TestText", false)]
        [InlineData("TestKey", "TestText", "BadKey", null, "...", true)]
        [InlineData("TestKey", "TestText_{0}", "TestKey", "MyArg", "TestText_MyArg", false)]
        [InlineData("TestKey", "TestText_{0}", "BadKey", "MyArg", "...", true)]
        public async Task ItShouldLoadLocalizerDataAsync(
            string resourceKey, string resourceText,
            string queryKey, string queryArg,
            string expectedText, bool expectedNotFound)
        {
            var taskCompletionSource = new TaskCompletionSource();

            var localizer = CreateLocalizerAsync(resourceKey, resourceText, taskCompletionSource);

            Assert.False(localizer.IsLoaded);

            // Make sure the localized text is loading...
            var loading = string.IsNullOrEmpty(queryArg)
                ? localizer[queryKey]
                : localizer[queryKey, queryArg];

            Assert.Equal("...", loading.Value);

            // Signal the async task.
            taskCompletionSource.SetResult();

            // Make sure the localized text is loaded.
            var loaded = await AStringLocalizerInternal.LoadAsync(localizer, false);

            Assert.True(loaded, "Could not load resources.");

            Assert.True(localizer.IsLoaded);

            // Make sure the localized text is the one expected.
            var value = string.IsNullOrEmpty(queryArg)
                ? localizer[queryKey]
                : localizer[queryKey, queryArg];

            Assert.Equal(expectedText, value.Value);
            Assert.Equal(expectedNotFound, value.ResourceNotFound);
        }

        private static JsonStringLocalizerAsync CreateLocalizerAsync(string key, string txt, TaskCompletionSource taskCompletionSource, string cultureName = "en-us")
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var resourceSource = new StringLocalizerResourceSource("test", typeof(ConstStringLocalizerTest).Assembly, null);

            async Task<IReadOnlyDictionary<string, string>?> loadAsync()
            {
                // Wait until signal.
                await taskCompletionSource.Task.ConfigureAwait(false);

                // Return data
                return new Dictionary<string, string>()
                {
                    [key] = txt
                };
            }

            var localizerFactoryInternalMock = new Mock<IJsonStringLocalizerFactoryInternal>();

            localizerFactoryInternalMock
                .Setup(x => x.LoadDataThroughStringLocalizerHierarchyAsync(
                    It.IsAny<IStringLocalizerInternal>(),
                    It.IsAny<CultureInfo>(),
                    It.IsAny<bool>()))
                .Returns<IStringLocalizerInternal, CultureInfo, bool>(async (localizer, ci, loadParent) =>
                {
                    await localizer.LoadDataAsync().ConfigureAwait(false);
                });


            localizerFactoryInternalMock
                .Setup(x => x.FindThroughStringLocalizerHierarchy(
                    It.IsAny<IStringLocalizerInternal>(),
                    It.IsAny<CultureInfo>(),
                    It.IsAny<Func<IStringLocalizerInternal, LocalizedString?>>()))
                .Returns<IStringLocalizerInternal, CultureInfo, Func<IStringLocalizerInternal, LocalizedString?>>((localizer, ci, forward) =>
                {
                    return forward(localizer);
                });

            localizerFactoryInternalMock
                .Setup(x => x.CreateDefaultStringLocalizer(
                    It.IsAny<StringLocalizerResourceSource>(),
                    It.IsAny<CultureInfo>(),
                    It.IsAny<string>()))
                .Returns<StringLocalizerResourceSource, CultureInfo, string>((source, ci, id) =>
                {
                    return new ConstStringLocalizer(source, ci, localizerFactoryInternalMock.Object, "...", true, id);
                });


            // Setup the async localizer with the loadAsync task.
            var localizer = new JsonStringLocalizerAsync(
                resourceSource,
                loadAsync(),
                cultureInfo,
                localizerFactoryInternalMock.Object);
            return localizer;
        }
    }
}
