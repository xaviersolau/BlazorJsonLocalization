// ----------------------------------------------------------------------
// <copyright file="StringLocalizerProxyTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using Moq;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Services;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class StringLocalizerProxyTest
    {
        private static readonly string BaseName = typeof(StringLocalizerProxyTest).FullName ?? nameof(StringLocalizerProxyTest);
        private static readonly Assembly Assembly = typeof(StringLocalizerProxyTest).Assembly;

        [Fact]
        public void ItShould()
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

            var proxy = new StringLocalizerProxy(cultureInfoServiceMock.Object,
                (name, asm, ci) => map[ci.Name],
                Assembly,
                BaseName);

            Assert.Same(stringLocalizerEn, proxy.CurrentStringLocalizer);

            currentCulture = cultureInfoFr;

            Assert.Same(stringLocalizerFr, proxy.CurrentStringLocalizer);
        }
    }
}
