// ----------------------------------------------------------------------
// <copyright file="StringLocalizerProxyTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.CodeQuality.Test.Helpers.XUnit.Logger;
using System.Collections.Generic;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class StringLocalizerProxyTest
    {
        private ILogger<StringLocalizerProxy> Logger { get; }
        public StringLocalizerProxyTest(ITestOutputHelper testOutputHelper)
        {
            Logger = new TestLogger<StringLocalizerProxy>(testOutputHelper);
        }

        [Fact]
        public void ItShouldProvideTheRightStringLocalizerDependingOnTheCurrentCulture()
        {
            var cultureInfoFr = CultureInfo.GetCultureInfo("fr-FR");
            var cultureInfoEn = CultureInfo.GetCultureInfo("en-US");

            var stringLocalizerFr = Mock.Of<IStringLocalizer>();
            var stringLocalizerEn = Mock.Of<IStringLocalizer>();

            var map = new Dictionary<string, IStringLocalizer>()
            {
                [cultureInfoFr.Name] = stringLocalizerFr,
                [cultureInfoEn.Name] = stringLocalizerEn,
            };

            var currentCulture = cultureInfoEn;

            var cultureInfoServiceMock = new Mock<ICultureInfoService>();
            cultureInfoServiceMock.SetupGet(s => s.CurrentUICulture).Returns(() => currentCulture);

            var localizerFactoryMock = new Mock<IJsonStringLocalizerFactoryInternal>();
            localizerFactoryMock
                .Setup(x => x.CreateStringLocalizer(It.IsAny<CultureInfo>()))
                .Returns<CultureInfo>(ci => map[ci.Name]);

            var proxy = new StringLocalizerProxy(
                Logger,
                cultureInfoServiceMock.Object,
                localizerFactoryMock.Object
                );

            Assert.Same(stringLocalizerEn, proxy.CurrentStringLocalizer);

            currentCulture = cultureInfoFr;

            Assert.Same(stringLocalizerFr, proxy.CurrentStringLocalizer);
        }
    }
}
