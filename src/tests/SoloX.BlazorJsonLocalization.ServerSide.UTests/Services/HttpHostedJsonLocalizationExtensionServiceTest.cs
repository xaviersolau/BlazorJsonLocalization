// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionServiceTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Moq;
using SoloX.BlazorJsonLocalization.ServerSide.Services.Impl;
using SoloX.CodeQuality.Test.Helpers.XUnit.Logger;
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
            string expectedText,
            bool expectedSuccess,
            string resourcePath)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"English test.\"}"));
            using var streamFr = new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"French test.\"}"));

            var fileInfoMock = new Mock<IFileInfo>();
            fileInfoMock.SetupGet(f => f.Exists).Returns(true);
            fileInfoMock.Setup(f => f.CreateReadStream()).Returns(stream);

            var fileInfoFrMock = new Mock<IFileInfo>();
            fileInfoFrMock.SetupGet(f => f.Exists).Returns(true);
            fileInfoFrMock.Setup(f => f.CreateReadStream()).Returns(streamFr);

            var fileInfoNoneMock = new Mock<IFileInfo>();
            fileInfoNoneMock.SetupGet(f => f.Exists).Returns(false);

            var fileProviderMock = new Mock<IFileProvider>();
            fileProviderMock
                .Setup(p => p.GetFileInfo(It.IsAny<string>()))
                .Returns(fileInfoNoneMock.Object);
            fileProviderMock
                .Setup(p => p.GetFileInfo("_content/SoloX.BlazorJsonLocalization.ServerSide.UTests/Resources/HttpHostedJsonLocalizationExtensionServiceTest.json"))
                .Returns(fileInfoMock.Object);
            fileProviderMock
                .Setup(p => p.GetFileInfo("_content/SoloX.BlazorJsonLocalization.ServerSide.UTests/Resources/HttpHostedJsonLocalizationExtensionServiceTest-fr.json"))
                .Returns(fileInfoFrMock.Object);

            var hostEnvMock = new Mock<IWebHostEnvironment>();
            hostEnvMock.SetupGet(x => x.WebRootFileProvider).Returns(fileProviderMock.Object);

            var service = new HttpHostedJsonLocalizationExtensionService(hostEnvMock.Object, Logger);

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
