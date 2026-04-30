// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerAsyncTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core.Impl;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using SoloX.BlazorJsonLocalization.Core;
using Microsoft.Extensions.Localization;
using System;
using NSubstitute.Core;

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
            string queryKey, string? queryArg,
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

            var localizerFactoryInternalMock = Substitute.For<IJsonStringLocalizerFactoryInternal>();

#pragma warning disable CA2012 // Use ValueTasks correctly
            localizerFactoryInternalMock
                .LoadDataThroughStringLocalizerHierarchyAsync(
                    Arg.Any<IStringLocalizerInternal>(),
                    Arg.Any<CultureInfo>(),
                    Arg.Any<bool>())
                .Returns((Func<CallInfo, ValueTask>)(async ci =>
                {
                    var localizer = ci.Arg<IStringLocalizerInternal>();
                    await localizer.LoadDataAsync().ConfigureAwait(false);
                }));
#pragma warning restore CA2012 // Use ValueTasks correctly


            localizerFactoryInternalMock
                .FindThroughStringLocalizerHierarchy(
                    Arg.Any<IStringLocalizerInternal>(),
                    Arg.Any<CultureInfo>(),
                    Arg.Any<Func<IStringLocalizerInternal, LocalizedString?>>())
                .Returns(ci =>
                {
                    var localizer = ci.Arg<IStringLocalizerInternal>();
                    var cInfo = ci.Arg<CultureInfo>();
                    var forward = ci.Arg<Func<IStringLocalizerInternal, LocalizedString?>>();
                    return forward(localizer);
                });

            localizerFactoryInternalMock
                .CreateDefaultStringLocalizer(
                    Arg.Any<StringLocalizerResourceSource>(),
                    Arg.Any<CultureInfo>(),
                    Arg.Any<string>())
                .Returns(ci =>
                {
                    var source = ci.Arg<StringLocalizerResourceSource>();
                    var cInfo = ci.Arg<CultureInfo>();
                    var id = ci.Arg<string>();
                    return new ConstStringLocalizer(source, cInfo, localizerFactoryInternalMock, "...", true, id);
                });


            // Setup the async localizer with the loadAsync task.
            var localizer = new JsonStringLocalizerAsync(
                resourceSource,
                loadAsync(),
                cultureInfo,
                localizerFactoryInternalMock);

            return localizer;
        }
    }
}
