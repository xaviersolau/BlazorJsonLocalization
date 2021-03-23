// ----------------------------------------------------------------------
// <copyright file="JsonLocalyserSetupTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using SoloX.BlazorJsonLocalization.Services;
using System.Globalization;
using Xunit;

namespace SoloX.BlazorJsonLocalization.ITests
{
    public class JsonLocalyserSetupTest
    {
        [Theory]
        [InlineData("fr-FR", "Test", null, "C'est un test...")]
        [InlineData("en-US", "Test", null, "This is a test...")]
        [InlineData("fr-FR", "TestWithArg", "arg", "C'est un test avec un argument: arg...")]
        [InlineData("en-US", "TestWithArg", "arg", "This is a test with an argument: arg...")]
        public void ItShouldSetupEmbeddedLocalizer(string cultureName, string key, string arg, string expected)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var cultureInfoServiceMock = new Mock<ICultureInfoService>();

            cultureInfoServiceMock.SetupGet(s => s.CurrentUICulture).Returns(cultureInfo);

            var services = new ServiceCollection();

            services.AddJsonLocalization(
                builder => builder.UseEmbeddedJson(options => options.ResourcesPath = "Resources"));

            services.AddSingleton(cultureInfoServiceMock.Object);

            using var provider = services.BuildServiceProvider();

            var localizer = provider.GetService<IStringLocalizer<JsonLocalyserSetupTest>>();

            Assert.NotNull(localizer);

            var localized = string.IsNullOrEmpty(arg)
                ? localizer[key]
                : localizer[key, arg];

            Assert.Equal(expected, localized.Value);
        }
    }
}
