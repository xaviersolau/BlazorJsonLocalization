// ----------------------------------------------------------------------
// <copyright file="StringLocalizerProxyTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Moq;
using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Services;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class StringLocalizerProxyTest
    {
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
                .Setup(x => x.CreateStringLocalizer(It.IsAny<CultureInfo>()))
                .Returns<CultureInfo>(ci => map[ci.Name]);

            var proxy = new StringLocalizerProxy(cultureInfoServiceMock.Object,
                localizerFactoryMock.Object);

            Assert.Same(stringLocalizerEn, proxy.CurrentStringLocalizer);

            currentCulture = cultureInfoFr;

            Assert.Same(stringLocalizerFr, proxy.CurrentStringLocalizer);
        }
    }
}
