// ----------------------------------------------------------------------
// <copyright file="JsonLocalyserWebAssemblySetupTest.cs" company="Xavier Solau">
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
using SoloX.CodeQuality.Test.Helpers.XUnit;
using SoloX.CodeQuality.Test.Helpers.Http;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using SoloX.BlazorJsonLocalization.Http;

namespace SoloX.BlazorJsonLocalization.ITests
{
    public class JsonLocalyserWebAssemblySetupTest
    {
        private readonly ITestOutputHelper testOutputHelper;
        public JsonLocalyserWebAssemblySetupTest(ITestOutputHelper testOutputHelper)
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

            using var httpClient = new HttpClientMockBuilder()
                .WithBaseAddress(new Uri("http://test.com"))
                .WithRequest("/_content/SoloX.BlazorJsonLocalization.ITests/Resources/JsonLocalyserWebAssemblySetupTest.json")
                .Responding(request =>
                {
                    var response = new HttpResponseMessage();
                    response.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"This is a test...\", \"TestWithArg\": \"This is a test with an argument: {0}...\"}")));
                    return response;
                })
                .WithRequest("/_content/SoloX.BlazorJsonLocalization.ITests/Resources/JsonLocalyserWebAssemblySetupTest-fr.json")
                .Responding(request =>
                {
                    var response = new HttpResponseMessage();
                    response.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\": \"C'est un test...\", \"TestWithArg\": \"C'est un test avec un argument: {0}...\"}")));
                    return response;
                })
                .Build();

            services.AddSingleton(httpClient);

            services.AddWebAssemblyJsonLocalization(
                builder => builder.UseHttpHostedJson(options => options.ResourcesPath = "Resources"));

            services.AddSingleton(cultureInfoServiceMock.Object);

            using var provider = services.BuildServiceProvider();

            var localizer = provider.GetService<IStringLocalizer<JsonLocalyserWebAssemblySetupTest>>();

            Assert.NotNull(localizer);

            await localizer.LoadAsync().ConfigureAwait(false);

            var localized = string.IsNullOrEmpty(arg)
                ? localizer[key]
                : localizer[key, arg];

            Assert.Equal(expected, localized.Value);
        }
    }
}
