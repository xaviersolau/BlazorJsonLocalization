// ----------------------------------------------------------------------
// <copyright file="SubStringLocalizerTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using SoloX.BlazorJsonLocalization.Core.Impl;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class SubStringLocalizerTest
    {
        [Fact]
        public void IsShouldForwardToParentLocalizerWithPrefix()
        {
            var localizerFactoryMock = new Mock<IStringLocalizer>();

            var keyName = "KeyPrefix:SomeInput";
            var text = "Some constant text...";
            localizerFactoryMock.SetupGet(l => l[keyName]).Returns(new LocalizedString(keyName, text));

            var localizer = new SubStringLocalizer(localizerFactoryMock.Object, "KeyPrefix:");

            Assert.Equal(text, localizer["SomeInput"]);
        }

        [Fact]
        public void IsShouldForwardToParentLocalizerWithPrefixAndArguments()
        {
            var localizerFactoryMock = new Mock<IStringLocalizer>();

            var keyName = "KeyPrefix:SomeInput";
            var text = "Some constant text...";
            var arg = "Argument";
            localizerFactoryMock.SetupGet(l => l[keyName, arg]).Returns(new LocalizedString(keyName, text));

            var localizer = new SubStringLocalizer(localizerFactoryMock.Object, "KeyPrefix:");

            Assert.Equal(text, localizer["SomeInput", arg]);
        }

        [Fact]
        public void IsShouldForwardToParentLocalizerGetAllStrings()
        {
            var localizerFactoryMock = new Mock<IStringLocalizer>();

            var keyName = "KeyPrefix:SomeInput";
            var text = "Some constant text...";
            var arg = "Argument";

            var localizedString = new LocalizedString(keyName, text);

            localizerFactoryMock.Setup(l => l.GetAllStrings(It.IsAny<bool>())).Returns(new LocalizedString[] { localizedString });

            var localizer = new SubStringLocalizer(localizerFactoryMock.Object, "KeyPrefix:");

            var allStrings = localizer.GetAllStrings(false);

            allStrings.Should().ContainSingle().Which.Should().Be(localizedString);
        }
    }
}
