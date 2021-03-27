// ----------------------------------------------------------------------
// <copyright file="JsonLocalyserServerSideSetupTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.ServerSide;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Text;
using Xunit.Abstractions;
using SoloX.CodeQuality.Test.Helpers;

namespace SoloX.BlazorJsonLocalization.ITests
{
    public class JsonLocalyserServerSideSetupTest
    {
        private readonly ITestOutputHelper testOutputHelper;
        public JsonLocalyserServerSideSetupTest(ITestOutputHelper testOutputHelper)
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
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var cultureInfoServiceMock = new Mock<ICultureInfoService>();

            cultureInfoServiceMock.SetupGet(s => s.CurrentUICulture).Returns(cultureInfo);

            var services = new ServiceCollection();
            services.AddTestLogging(this.testOutputHelper);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"This is a test...\", \"TestWithArg\": \"This is a test with an argument: {0}...\"}"));
            using var streamFr = new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"C'est un test...\", \"TestWithArg\": \"C'est un test avec un argument: {0}...\"}"));

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
                .Setup(p => p.GetFileInfo("_content/SoloX.BlazorJsonLocalization.ITests/Resources/JsonLocalyserServerSideSetupTest.json"))
                .Returns(fileInfoMock.Object);
            fileProviderMock
                .Setup(p => p.GetFileInfo("_content/SoloX.BlazorJsonLocalization.ITests/Resources/JsonLocalyserServerSideSetupTest-fr.json"))
                .Returns(fileInfoFrMock.Object);

            var hostEnvMock = new Mock<IWebHostEnvironment>();
            hostEnvMock.SetupGet(x => x.WebRootFileProvider).Returns(fileProviderMock.Object);

            services.AddSingleton(hostEnvMock.Object);

            services.AddServerSideJsonLocalization(
                builder => builder.UseHttpHostedJson(options => options.ResourcesPath = "Resources"));

            services.AddSingleton(cultureInfoServiceMock.Object);

            using var provider = services.BuildServiceProvider();

            var localizer = provider.GetService<IStringLocalizer<JsonLocalyserServerSideSetupTest>>();

            Assert.NotNull(localizer);

            await localizer.LoadAsync().ConfigureAwait(false);

            var localized = string.IsNullOrEmpty(arg)
                ? localizer[key]
                : localizer[key, arg];

            Assert.Equal(expected, localized.Value);
        }
    }
}
