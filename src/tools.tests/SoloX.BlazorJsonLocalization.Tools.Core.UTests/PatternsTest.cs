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
            var valueName = nameof(IMyObjectStringLocalizerPattern.SomeStringArgs);

            var argument = "My Argument";

            this.mock
                .Setup(x => x[nameof(IMyObjectStringLocalizerPattern.SomeStringArgs), argument])
                .Returns(new LocalizedString(valueName, expectedValue));

            var value = this.pattern.SomeStringArgs(argument);

            value.Should().Be(expectedValue);
        }

        [Fact]
        public void IsShouldForwardPropertyGetToStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var propertyName = nameof(IMyObjectStringLocalizerPattern.SomeProperty);

            this.mock
                .Setup(x => x[nameof(IMyObjectStringLocalizerPattern.SomeProperty)])
                .Returns(new LocalizedString(propertyName, expectedValue));

            var value = this.pattern.SomeProperty;

            value.Should().Be(expectedValue);
        }

        [Fact]
        public void IsShouldForwardSubPropertyGetToRootStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var propertyName = nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty);

            this.mock
                .Setup(x => x[typeof(IMyObjectSubStringLocalizerPattern).FullName + ":" + nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty), Array.Empty<object>()])
                .Returns(new LocalizedString(propertyName, expectedValue));

            var value = this.pattern.MyObjectSubStringLocalizerProperty.SomeSubProperty;

            value.Should().Be(expectedValue);
        }

        [Fact]
        public void IsShouldForwardSubArgumentGetToRelativeStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var argName = nameof(IMyObjectSubStringLocalizerPattern.SomeArgument);

            this.mock
                .Setup(x => x[nameof(IMyObjectStringLocalizerPattern.MyObjectSubStringLocalizerProperty) + ":" + nameof(IMyObjectSubStringLocalizerPattern.SomeArgument)])
                .Returns(new LocalizedString(argName, expectedValue));

            var value = this.pattern.MyObjectSubStringLocalizerProperty.SomeArgument;

            value.Should().Be(expectedValue);
        }

        [Fact]
        public void IsShouldForwardSubPropertyGetToRootStringLocalizerAndReplaceArgument()
        {
            var propertyValue = "My Test Value Using The SomeArgument";
            var propertyName = nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty);

            var argValue = "My Arg Value";
            var argName = nameof(IMyObjectSubStringLocalizerPattern.SomeArgument);

            var expectedValue = "My Test Value Using The My Arg Value";

            this.mock
                .Setup(x => x[typeof(IMyObjectSubStringLocalizerPattern).FullName + ":" + nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty), Array.Empty<object>()])
                .Returns(new LocalizedString(propertyName, propertyValue));

            this.mock
                .Setup(x => x[nameof(IMyObjectStringLocalizerPattern.MyObjectSubStringLocalizerProperty) + ":" + nameof(IMyObjectSubStringLocalizerPattern.SomeArgument)])
                .Returns(new LocalizedString(argName, argValue));

            var value = this.pattern.MyObjectSubStringLocalizerProperty.SomeSubProperty;

            value.Should().Be(expectedValue);
        }
    }
}
