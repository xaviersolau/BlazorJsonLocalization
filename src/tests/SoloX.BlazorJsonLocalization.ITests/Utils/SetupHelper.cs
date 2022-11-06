// ----------------------------------------------------------------------
// <copyright file="SetupHelper.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.WebAssembly;
using SoloX.CodeQuality.Test.Helpers.Http;
using SoloX.CodeQuality.Test.Helpers.XUnit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.ITests.Utils
{
    public static class SetupHelper
    {
        internal static Task ProcessLocalizerTestAsync<T>(
            string cultureName,
            Func<IStringLocalizer<T>, Task> testHandler,
            ITestOutputHelper testOutputHelper,
            Action<JsonLocalizationOptionsBuilder>? builderHandler = null)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var cultureInfoServiceMock = new Mock<ICultureInfoService>();

            cultureInfoServiceMock.SetupGet(s => s.CurrentUICulture).Returns(cultureInfo);

            var services = new ServiceCollection();
            services.AddTestLogging(testOutputHelper);

            services.AddJsonLocalization(
                builder =>
                {
                    builder.UseEmbeddedJson(options => options.ResourcesPath = "Resources");

                    if (builderHandler != null)
                    {
                        builderHandler(builder);
                    }
                });

            services.AddSingleton(cultureInfoServiceMock.Object);
            using var provider = services.BuildServiceProvider();
            var localizer = provider.GetRequiredService<IStringLocalizer<T>>();

            return testHandler(localizer);
        }

        internal static Task ProcessHttpLocalizerTestAsync<T>(
            string cultureName,
            IDictionary<string, string> httpResources,
            Func<IStringLocalizer<T>, Task> testHandler,
            ITestOutputHelper testOutputHelper,
            Action<JsonLocalizationOptionsBuilder>? builderHandler = null)
        {
            return ProcessHttpLocalizerTestAsync<T>(
                cultureName,
                httpResources,
                (l, unlocker) =>
                {
                    foreach (var kvp in httpResources)
                    {
                        unlocker(kvp.Key);
                    }

                    return testHandler(l);
                },
                testOutputHelper,
                builderHandler);
        }

        internal static async Task ProcessHttpLocalizerTestAsync<T>(
            string cultureName,
            IDictionary<string, string> httpResources,
            Func<IStringLocalizer<T>, Action<string>, Task> testHandler,
            ITestOutputHelper testOutputHelper,
            Action<JsonLocalizationOptionsBuilder>? builderHandler = null)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var cultureInfoServiceMock = new Mock<ICultureInfoService>();

            cultureInfoServiceMock.SetupGet(s => s.CurrentUICulture).Returns(cultureInfo);

            var services = new ServiceCollection();
            services.AddTestLogging(testOutputHelper);

            var httpClientBuilder = new HttpClientMockBuilder()
                .WithBaseAddress(new Uri("http://test.com"));

            var taskCompletionSourceMap = new Dictionary<string, TaskCompletionSource>();

            foreach (var httpResource in httpResources)
            {
                var taskCompletionSource = new TaskCompletionSource();
                taskCompletionSourceMap.Add(httpResource.Key, taskCompletionSource);

                httpClientBuilder = httpClientBuilder.WithRequest($"/_content/SoloX.BlazorJsonLocalization.ITests/Resources/{httpResource.Key}")
                    .Responding(async request =>
                    {
                        await taskCompletionSource.Task.ConfigureAwait(false);

                        var response = new HttpResponseMessage();
                        response.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(httpResource.Value)));
                        return response;
                    });
            }

            using var httpClient = httpClientBuilder.Build();

            services.AddSingleton(httpClient);

            services.AddWebAssemblyJsonLocalization(
                builder =>
                {
                    builder.UseHttpHostedJson(options => options.ResourcesPath = "Resources");

                    if (builderHandler != null)
                    {
                        builderHandler(builder);
                    }
                });

            services.AddSingleton(cultureInfoServiceMock.Object);

            await using var provider = services.BuildServiceProvider();
            var localizer = provider.GetRequiredService<IStringLocalizer<T>>();

            await testHandler(localizer, s => taskCompletionSourceMap[s].SetResult()).ConfigureAwait(false);
        }
    }
}
