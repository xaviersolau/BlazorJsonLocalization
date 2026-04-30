// ----------------------------------------------------------------------
// <copyright file="SubStringLocalizerTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Localization;
using NSubstitute;
using SoloX.BlazorJsonLocalization.Core.Impl;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class SubStringLocalizerTest
    {
        [Fact]
        public void IsShouldForwardToParentLocalizerWithPrefix()
        {
            var localizerFactoryMock = Substitute.For<IStringLocalizer>();

            var keyName = "KeyPrefix:SomeInput";
            var text = "Some constant text...";
            localizerFactoryMock[keyName].Returns(new LocalizedString(keyName, text));

            var localizer = new SubStringLocalizer(localizerFactoryMock, "KeyPrefix:");

            Assert.Equal(text, localizer["SomeInput"]);
        }

        [Fact]
        public void IsShouldForwardToParentLocalizerWithPrefixAndArguments()
        {
            var localizerFactoryMock = Substitute.For<IStringLocalizer>();

            var keyName = "KeyPrefix:SomeInput";
            var text = "Some constant text...";
            var arg = "Argument";
            localizerFactoryMock[keyName, arg].Returns(new LocalizedString(keyName, text));

            var localizer = new SubStringLocalizer(localizerFactoryMock, "KeyPrefix:");

            Assert.Equal(text, localizer["SomeInput", arg]);
        }

        [Fact]
        public void IsShouldForwardToParentLocalizerGetAllStrings()
        {
            var localizerFactoryMock = Substitute.For<IStringLocalizer>();

            var keyName = "KeyPrefix:SomeInput";
            var text = "Some constant text...";
            var arg = "Argument";

            var localizedString = new LocalizedString(keyName, text);

            localizerFactoryMock.GetAllStrings(Arg.Any<bool>()).Returns(new LocalizedString[] { localizedString });

            var localizer = new SubStringLocalizer(localizerFactoryMock, "KeyPrefix:");

            var allStrings = localizer.GetAllStrings(false);

            allStrings.Should().ContainSingle().Which.Should().Be(localizedString);
        }
    }
}
