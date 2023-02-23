// ----------------------------------------------------------------------
// <copyright file="PatternsTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;

namespace SoloX.BlazorJsonLocalization.Tools.Core.UTests
{
    public class PatternsTest
    {
        private readonly Mock<IStringLocalizer<MyObject>> mock;
        private readonly IMyObjectStringLocalizerPattern pattern;

        public PatternsTest()
        {
            this.mock = new Mock<IStringLocalizer<MyObject>>();
            this.pattern = this.mock.Object.ToMyObjectStringLocalizerPattern();
        }

        [Fact]
        public void IsShouldGetInterfaceImplementation()
        {
            this.pattern.Should().NotBeNull();
        }

        [Fact]
        public void IsShouldForwardMethodCallToStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var expectedName = "My Test Name";

            var argument = "My Argument";

            this.mock
                .Setup(x => x[nameof(IMyObjectStringLocalizerPattern.SomeStringArgs), argument])
                .Returns(new LocalizedString(expectedName, expectedValue));

            var value = this.pattern.SomeStringArgs(argument);

            value.Should().Be(expectedValue);
        }

        [Fact]
        public void IsShouldForwardPropertyGetToStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var expectedName = "My Test Name";

            this.mock
                .Setup(x => x[nameof(IMyObjectStringLocalizerPattern.SomeProperty)])
                .Returns(new LocalizedString(expectedName, expectedValue));

            var value = this.pattern.SomeProperty;

            value.Should().Be(expectedValue);
        }

        [Fact]
        public void IsShouldForwardSubPropertyGetToStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var expectedName = "My Test Name";

            this.mock
                .Setup(x => x[nameof(IMyObjectStringLocalizerPattern.MyObjectSubStringLocalizerProperty) + ":" + nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty)])
                .Returns(new LocalizedString(expectedName, expectedValue));

            var value = this.pattern.MyObjectSubStringLocalizerProperty.SomeSubProperty;

            value.Should().Be(expectedValue);
        }
    }
}
