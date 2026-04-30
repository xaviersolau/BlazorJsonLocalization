// ----------------------------------------------------------------------
// <copyright file="StringLocalizerProxyTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using NSubstitute;
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

            var stringLocalizerFr = Substitute.For<IStringLocalizerInternal>();
            var stringLocalizerEn = Substitute.For<IStringLocalizerInternal>();

            var map = new Dictionary<string, IStringLocalizerInternal>()
            {
                [cultureInfoFr.Name] = stringLocalizerFr,
                [cultureInfoEn.Name] = stringLocalizerEn,
            };

            var currentCulture = cultureInfoEn;

            var cultureInfoServiceMock = Substitute.For<ICultureInfoService>();
            cultureInfoServiceMock.CurrentUICulture.Returns((ci) => currentCulture);

            var localizerFactoryMock = Substitute.For<IJsonStringLocalizerFactoryInternal>();
            localizerFactoryMock
                .CreateStringLocalizer(Arg.Any<StringLocalizerResourceSource>(), Arg.Any<CultureInfo>())
                .Returns(ci =>
                {
                    var resourceSource = ci.Arg<StringLocalizerResourceSource>();
                    var culture = ci.Arg<CultureInfo>();
                    return map[culture.Name];
                });

            var resourceSource = new StringLocalizerResourceSource("test", typeof(ConstStringLocalizerTest).Assembly, null);
            var proxy = new StringLocalizerProxy(
                resourceSource,
                Logger,
                cultureInfoServiceMock,
                localizerFactoryMock
                );

            Assert.Same(stringLocalizerEn, proxy.CurrentStringLocalizer);

            currentCulture = cultureInfoFr;

            Assert.Same(stringLocalizerFr, proxy.CurrentStringLocalizer);
        }
    }
}
