// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionServiceTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.WebAssembly.Services.Impl;
using SoloX.CodeQuality.Test.Helpers.Http;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoloX.BlazorJsonLocalization.WebAssembly.UTests.Services
{
    public class HttpHostedJsonLocalizationExtensionServiceTest
    {
        private const string BaseName = nameof(HttpHostedJsonLocalizationExtensionServiceTest);

        private static readonly Assembly Assembly = typeof(HttpHostedJsonLocalizationExtensionServiceTest).Assembly;

        [Theory]
        [InlineData("en-US", "Test", "English test.", true, "Resources")]
        [InlineData("fr-FR", "Test", "French test.", true, "Resources")]
        [InlineData("en-US", "Test", null, false, "Bad")]
        [InlineData("fr-FR", "Test", null, false, "Bad")]
        [InlineData("en-US", "Test", null, false, null)]
        [InlineData("fr-FR", "Test", null, false, null)]
        public async Task ItShouldLoadJsonFileFromHttpClientAsync(
            string cultureName,
            string key,
            string expectedText,
            bool expectedSuccess,
            string resourcePath)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            using var httpClient = new HttpClientMockBuilder()
                .WithBaseAddress(new Uri("http://test.com"))
                .WithOkResponse(
                    "/_content/SoloX.BlazorJsonLocalization.WebAssembly.UTests/Resources/HttpHostedJsonLocalizationExtensionServiceTest.json",
                    request => new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"English test.\"}"))))
                .WithOkResponse(
                    "/_content/SoloX.BlazorJsonLocalization.WebAssembly.UTests/Resources/HttpHostedJsonLocalizationExtensionServiceTest-fr.json",
                    request => new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"French test.\"}"))))
                .Build();

            var service = new HttpHostedJsonLocalizationExtensionService(httpClient);

            var options = new HttpHostedJsonLocalizationOptions()
            {
                ResourcesPath = resourcePath,
            };

            var map = await service.TryLoadAsync(options, Assembly, BaseName, cultureInfo).ConfigureAwait(false);

            if (expectedSuccess)
            {
                Assert.NotNull(map);
                Assert.Equal(expectedText, map[key]);
            }
            else
            {
                Assert.Null(map);
            }
        }
    }
}
