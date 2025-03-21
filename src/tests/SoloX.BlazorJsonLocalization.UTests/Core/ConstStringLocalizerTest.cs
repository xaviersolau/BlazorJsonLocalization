﻿// ----------------------------------------------------------------------
// <copyright file="ConstStringLocalizerTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Globalization;
using Moq;
using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class ConstStringLocalizerTest
    {
        [Fact]
        public void IsShouldReturnTheGivenConstText()
        {
            var localizerFactoryMock = new Mock<IJsonStringLocalizerFactoryInternal>();
            var cultureInfo = CultureInfo.GetCultureInfo("en");

            var constText = "Some constant text...";
            var resourceSource = new StringLocalizerResourceSource("test", typeof(ConstStringLocalizerTest).Assembly, null);
            var localizer = new ConstStringLocalizer(resourceSource, cultureInfo, localizerFactoryMock.Object, constText, false, null);

            Assert.Equal(constText, localizer["SomeInput"]);
            Assert.Equal(constText, localizer["SomeInput", "With some argument"]);
            Assert.Equal(constText, localizer.TryGet("SomeInput")!);
            Assert.Equal(constText, localizer.TryGet("SomeInput", ["With some argument"], cultureInfo)!);
        }
    }
}
