// ----------------------------------------------------------------------
// <copyright file="PatternsTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Shouldly;
using Microsoft.Extensions.Localization;
using NSubstitute;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;

namespace SoloX.BlazorJsonLocalization.Tools.Core.UTests
{
    public class PatternsTest
    {
        private readonly IStringLocalizer<MyObject> mock;
        private readonly IMyObjectStringLocalizerPattern pattern;

        public PatternsTest()
        {
            this.mock = Substitute.For<IStringLocalizer<MyObject>>();
            this.pattern = this.mock.ToMyObjectStringLocalizerPattern();
        }

        [Fact]
        public void IsShouldGetInterfaceImplementation()
        {
            this.pattern.ShouldNotBeNull();
        }

        [Fact]
        public void IsShouldForwardMethodCallToStringLocalizer()
        {
            AssertForwardMethodCallToStringLocalizer((argument) => this.pattern.SomeStringArgs(argument));
        }

        [Fact]
        public void IsShouldForwardMethodCallToStringLocalizerUsingExtension()
        {
            AssertForwardMethodCallToStringLocalizer((argument) => this.mock.SomeStringArgs(argument));
        }

        public void AssertForwardMethodCallToStringLocalizer(Func<string, string> processWithArg)
        {
            var expectedValue = "My Test Value";
            var valueName = nameof(IMyObjectStringLocalizerPattern.SomeStringArgs);

            var argument = "My Argument";

            this.mock[nameof(IMyObjectStringLocalizerPattern.SomeStringArgs), argument]
                .Returns(new LocalizedString(valueName, expectedValue));

            var value = processWithArg(argument);

            value.ShouldBe(expectedValue);
        }

        [Fact]
        public void IsShouldForwardPropertyGetToStringLocalizer()
        {
            AssertForwardPropertyGetToStringLocalizer(() => this.pattern.SomeProperty);
        }

        [Fact]
        public void IsShouldForwardPropertyGetToStringLocalizerUsingExtension()
        {
            AssertForwardPropertyGetToStringLocalizer(() => this.mock.SomeProperty());
        }

        private void AssertForwardPropertyGetToStringLocalizer(Func<string> process)
        {
            var expectedValue = "My Test Value";
            var propertyName = nameof(IMyObjectStringLocalizerPattern.SomeProperty);

            this.mock[nameof(IMyObjectStringLocalizerPattern.SomeProperty)]
                .Returns(new LocalizedString(propertyName, expectedValue));

            var value = process();

            value.ShouldBe(expectedValue);
        }

        [Fact]
        public void IsShouldForwardSubPropertyGetToRootStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var propertyName = nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty);

            this.mock[typeof(IMyObjectSubStringLocalizerPattern).FullName + ":" + nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty), Array.Empty<object>()]
                .Returns(new LocalizedString(propertyName, expectedValue));

            var value = this.pattern.MyObjectSubStringLocalizerProperty.SomeSubProperty;

            value.ShouldBe(expectedValue);
        }

        [Fact]
        public void IsShouldForwardSubArgumentGetToRelativeStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var argName = nameof(IMyObjectSubStringLocalizerPattern.SomeArgument);

            this.mock[nameof(IMyObjectStringLocalizerPattern.MyObjectSubStringLocalizerProperty) + ":" + nameof(IMyObjectSubStringLocalizerPattern.SomeArgument)]
                .Returns(new LocalizedString(argName, expectedValue));

            var value = this.pattern.MyObjectSubStringLocalizerProperty.SomeArgument;

            value.ShouldBe(expectedValue);
        }

        [Fact]
        public void IsShouldForwardSubPropertyGetToRootStringLocalizerAndReplaceArgument()
        {
            var propertyValue = "My Test Value Using The SomeArgument";
            var propertyName = nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty);

            var argValue = "My Arg Value";
            var argName = nameof(IMyObjectSubStringLocalizerPattern.SomeArgument);

            var expectedValue = "My Test Value Using The My Arg Value";

            this.mock[typeof(IMyObjectSubStringLocalizerPattern).FullName + ":" + nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty), Array.Empty<object>()]
                .Returns(new LocalizedString(propertyName, propertyValue));

            this.mock[nameof(IMyObjectStringLocalizerPattern.MyObjectSubStringLocalizerProperty) + ":" + nameof(IMyObjectSubStringLocalizerPattern.SomeArgument)]
                .Returns(new LocalizedString(argName, argValue));

            var value = this.pattern.MyObjectSubStringLocalizerProperty.SomeSubProperty;

            value.ShouldBe(expectedValue);
        }
    }
}
