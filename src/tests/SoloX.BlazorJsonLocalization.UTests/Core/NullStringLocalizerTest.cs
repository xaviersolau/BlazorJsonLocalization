// ----------------------------------------------------------------------
// <copyright file="NullStringLocalizerTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core.Impl;
using System.Globalization;
using Moq;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class NullStringLocalizerTest
    {
        [Fact]
        public void IsShouldReturnTheGivenKeyAsText()
        {
            var cultureInfo = CultureInfo.GetCultureInfo("fr-FR");

            var localizerFactoryMock = new Mock<IJsonStringLocalizerFactoryInternal>();

            var localizer = new NullStringLocalizer(cultureInfo, localizerFactoryMock.Object);

            Assert.Equal("SomeInput", localizer["SomeInput"]);
            Assert.Equal("SomeInput", localizer["SomeInput", "With some argument"]);
        }
    }
}
