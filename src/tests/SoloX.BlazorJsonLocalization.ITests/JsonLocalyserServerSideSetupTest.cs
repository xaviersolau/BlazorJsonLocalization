// ----------------------------------------------------------------------
// <copyright file="JsonLocalyserServerSideSetupTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using NSubstitute;
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
using SoloX.CodeQuality.Test.Helpers.XUnit;

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
        public async Task ItShouldSetupServerSideHttpHostedLocalizerAsync(string cultureName, string key, string? arg, string expected)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var cultureInfoServiceMock = Substitute.For<ICultureInfoService>();

            cultureInfoServiceMock.CurrentUICulture.Returns(cultureInfo);

            var services = new ServiceCollection();
            services.AddTestLogging(this.testOutputHelper);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"This is a test...\", \"TestWithArg\": \"This is a test with an argument: {0}...\"}"));
            using var streamFr = new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"C'est un test...\", \"TestWithArg\": \"C'est un test avec un argument: {0}...\"}"));

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
                .GetFileInfo("_content/SoloX.BlazorJsonLocalization.ITests/Resources/JsonLocalyserServerSideSetupTest.json")
                .Returns(fileInfoMock);
            fileProviderMock
                .GetFileInfo("_content/SoloX.BlazorJsonLocalization.ITests/Resources/JsonLocalyserServerSideSetupTest.fr.json")
                .Returns(fileInfoFrMock);

            var hostEnvMock = Substitute.For<IWebHostEnvironment>();
            hostEnvMock.WebRootFileProvider.Returns(fileProviderMock);

            services.AddSingleton(hostEnvMock);

            services.AddServerSideJsonLocalization(
                builder => builder.UseHttpHostedJson(options => options.ResourcesPath = "Resources"));

            services.AddSingleton(cultureInfoServiceMock);

            using var provider = services.BuildServiceProvider();

            var localizer = provider.GetService<IStringLocalizer<JsonLocalyserServerSideSetupTest>>();

            Assert.NotNull(localizer);

            await localizer.LoadAsync();

            var localized = string.IsNullOrEmpty(arg)
                ? localizer[key]
                : localizer[key, arg];

            Assert.Equal(expected, localized.Value);
        }
    }
}
