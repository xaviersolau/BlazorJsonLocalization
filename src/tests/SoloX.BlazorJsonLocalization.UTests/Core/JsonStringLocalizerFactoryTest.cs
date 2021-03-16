// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerFactoryTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Options;
using Moq;
using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Services;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class JsonStringLocalizerFactoryTest
    {
        [Fact]
        public void IsShouldCreateAJsonStringLocalizerMatchingTheGivenBaseNameAndLocation()
        {
            var baseName = "basename";
            var key = "key";
            var value = "value";
            var map = new Dictionary<string, string>
            {
                [key] = value,
            };

            // Setup CultureInfo service mock.
            var cultureInfoServiceMock = new Mock<ICultureInfoService>();
            var cultureName = "en-us";
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            cultureInfoServiceMock
                .SetupGet(s => s.CurrentUICulture)
                .Returns(cultureInfo);

            var extensionServiceMock = new Mock<IJsonLocalizationExtensionService>();

            var extensionOptionsContainerMock = new Mock<IExtensionOptionsContainer>();
            var extensionOptionsContainer = extensionOptionsContainerMock.Object;

            // Setup extension resolver service.
            var extensionResolverServiceMock = new Mock<IExtensionResolverService>();
            extensionResolverServiceMock.Setup(s => s.GetExtensionService(extensionOptionsContainer))
                .Returns(extensionServiceMock.Object);

            var opt = new JsonLocalizationOptions();
            opt.ExtensionOptions = new IExtensionOptionsContainer[] { extensionOptionsContainer };

            var optionsMock = new Mock<IOptions<JsonLocalizationOptions>>();

            optionsMock
                .SetupGet(o => o.Value)
                .Returns(opt);

            var assembly = this.GetType().Assembly;
            extensionServiceMock
                .Setup(s => s.TryLoad(extensionOptionsContainer, assembly, baseName, cultureInfo))
                .Returns(map);

            var factory = new JsonStringLocalizerFactory(
                optionsMock.Object,
                cultureInfoServiceMock.Object,
                extensionResolverServiceMock.Object);

            var localizer = factory.Create(baseName, assembly.FullName);

            Assert.NotNull(localizer);

            Assert.Equal(value, localizer[key]);
        }
    }
}
