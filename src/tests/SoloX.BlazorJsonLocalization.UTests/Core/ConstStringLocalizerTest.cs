// ----------------------------------------------------------------------
// <copyright file="ConstStringLocalizerTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

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

            var constText = "Some constant text...";
            var localizer = new ConstStringLocalizer(constText, localizerFactoryMock.Object);

            Assert.Equal(constText, localizer["SomeInput"]);
            Assert.Equal(constText, localizer["SomeInput", "With some argument"]);
        }
    }
}
