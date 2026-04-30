// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionServiceTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SoloX.BlazorJsonLocalization.ServerSide.Services.Impl;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.CodeQuality.Test.Helpers.XUnit.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.ServerSide.UTests.Services
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
            string? expectedText,
            bool expectedSuccess,
            string? resourcePath)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"English test.\"}"));
            using var streamFr = new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"French test.\"}"));

            var fileInfoMock = Substitute.For<IFileInfo>();
            fileInfoMock.Exists.Returns(true);
            fileInfoMock.CreateReadStream().Returns(stream);

            var fileInfoFrMock = Substitute.For<IFileInfo>();
            fileInfoFrMock.Exists.Returns(true);
            fileInfoFrMock.CreateReadStream().Returns(streamFr);

            var fileInfoNoneMock = Substitute.For<IFileInfo>();
            fileInfoNoneMock.Exists.Returns(false);

            var fileProviderMock = Substitute.For<IFileProvider>();
            fileProviderMock
                .GetFileInfo(Arg.Any<string>())
                .Returns(fileInfoNoneMock);
            fileProviderMock
                .GetFileInfo("_content/SoloX.BlazorJsonLocalization.ServerSide.UTests/Resources/HttpHostedJsonLocalizationExtensionServiceTest.json")
                .Returns(fileInfoMock);
            fileProviderMock
                .GetFileInfo("_content/SoloX.BlazorJsonLocalization.ServerSide.UTests/Resources/HttpHostedJsonLocalizationExtensionServiceTest.fr.json")
                .Returns(fileInfoFrMock);

            var httpCacheServiceMock = Substitute.For<IHttpCacheService>();
            httpCacheServiceMock.ProcessLoadingTask(Arg.Any<Uri>(), Arg.Any<Func<Task<IReadOnlyDictionary<string, string>?>>>())
                .Returns(ci =>
                {
                    var loader = ci.Arg<Func<Task<IReadOnlyDictionary<string, string>?>>>();
                    return loader();
                });

            var hostEnvMock = Substitute.For<IWebHostEnvironment>();
            hostEnvMock.WebRootFileProvider.Returns(fileProviderMock);
            var optionsMock = Substitute.For<IOptions<JsonLocalizationOptions>>();
            optionsMock.Value.Returns(new JsonLocalizationOptions());

            var service = new HttpHostedJsonLocalizationExtensionService(optionsMock, hostEnvMock, Logger, httpCacheServiceMock);

            var options = new HttpHostedJsonLocalizationOptions()
            {
                ResourcesPath = resourcePath,
            };

            var map = await service.TryLoadAsync(options, Assembly, BaseName, cultureInfo);

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
