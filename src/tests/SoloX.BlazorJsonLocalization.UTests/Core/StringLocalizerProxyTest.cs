// ----------------------------------------------------------------------
// <copyright file="StringLocalizerProxyTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Moq;
using SoloX.BlazorJsonLocalization.Core;
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

            var stringLocalizerFr = Mock.Of<IStringLocalizerInternal>();
            var stringLocalizerEn = Mock.Of<IStringLocalizerInternal>();

            var map = new Dictionary<string, IStringLocalizerInternal>()
            {
                [cultureInfoFr.Name] = stringLocalizerFr,
                [cultureInfoEn.Name] = stringLocalizerEn,
            };

            var currentCulture = cultureInfoEn;

            var cultureInfoServiceMock = new Mock<ICultureInfoService>();
            cultureInfoServiceMock.SetupGet(s => s.CurrentUICulture).Returns(() => currentCulture);

            var localizerFactoryMock = new Mock<IJsonStringLocalizerFactoryInternal>();
            localizerFactoryMock
                .Setup(x => x.CreateStringLocalizer(It.IsAny<StringLocalizerResourceSource>(), It.IsAny<CultureInfo>()))
                .Returns<StringLocalizerResourceSource, CultureInfo>((rs, ci) => map[ci.Name]);

            var resourceSource = new StringLocalizerResourceSource("test", typeof(ConstStringLocalizerTest).Assembly, null);
            var proxy = new StringLocalizerProxy(
                resourceSource,
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
