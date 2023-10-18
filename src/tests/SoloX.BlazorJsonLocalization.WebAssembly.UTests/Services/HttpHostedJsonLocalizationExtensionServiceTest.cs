// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionServiceTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.WebAssembly.Services.Impl;
using SoloX.CodeQuality.Test.Helpers.Http;
using SoloX.CodeQuality.Test.Helpers.XUnit.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.WebAssembly.UTests.Services
{
    public class HttpHostedJsonLocalizationExtensionServiceTest
    {
        private const string BaseName = nameof(HttpHostedJsonLocalizationExtensionServiceTest);

        private static readonly Assembly Assembly = typeof(HttpHostedJsonLocalizationExtensionServiceTest).Assembly;

        private ILogger<HttpHostedJsonLocalizationExtensionService> Logger { get; }

        public HttpHostedJsonLocalizationExtensionServiceTest(ITestOutputHelper testOutputHelper)
        {
            Logger = new TestLogger<HttpHostedJsonLocalizationExtensionService>(testOutputHelper);
        }

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
                .WithRequest("/_content/SoloX.BlazorJsonLocalization.WebAssembly.UTests/Resources/HttpHostedJsonLocalizationExtensionServiceTest.json")
                .Responding(request =>
                {
                    var response = new HttpResponseMessage();
                    response.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"English test.\"}")));
                    return response;
                })
                .WithRequest("/_content/SoloX.BlazorJsonLocalization.WebAssembly.UTests/Resources/HttpHostedJsonLocalizationExtensionServiceTest.fr.json")
                .Responding(request =>
                {
                    var response = new HttpResponseMessage();
                    response.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"French test.\"}")));
                    return response;
                })
                .Build();

            var httpCacheServiceMock = new Mock<IHttpCacheService>();
            httpCacheServiceMock.Setup(x => x.ProcessLoadingTask(It.IsAny<Uri>(), It.IsAny<Func<Task<IReadOnlyDictionary<string, string>?>>>()))
                .Returns<Uri, Func<Task<IReadOnlyDictionary<string, string>?>>>((uri, loader) => loader());

            var optionsMock = new Mock<IOptions<JsonLocalizationOptions>>();
            optionsMock.SetupGet(o => o.Value).Returns(new JsonLocalizationOptions());

            var service = new HttpHostedJsonLocalizationExtensionService(optionsMock.Object, httpClient, Logger, httpCacheServiceMock.Object);

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
