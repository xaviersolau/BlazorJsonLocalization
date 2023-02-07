using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Tools.Core.UTests
{
    public class PatternsTest
    {
        private readonly Mock<IStringLocalizer<MyObject>> mock;
        private readonly IMyObjectStringLocalizerPattern pattern;

        public PatternsTest()
        {
            this.mock = new Mock<IStringLocalizer<MyObject>>();
            this.pattern = mock.Object.ToMyObjectStringLocalizerPattern();
        }

        [Fact]
        public void IsShouldGetInterfaceImplementation()
        {
            pattern.Should().NotBeNull();
        }

        [Fact]
        public void IsShouldForwardMethodCallToStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var expectedName = "My Test Name";

            var argument = "My Argument";

            mock.Setup(x => x[nameof(IMyObjectStringLocalizerPattern.SomeStringArgs), argument]).Returns(new LocalizedString(expectedName, expectedValue));

            var value = pattern.SomeStringArgs(argument);

            value.Should().Be(expectedValue);
        }

        [Fact]
        public void IsShouldForwardPropertyGetToStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var expectedName = "My Test Name";

            mock.Setup(x => x[nameof(IMyObjectStringLocalizerPattern.SomeProperty)]).Returns(new LocalizedString(expectedName, expectedValue));

            var value = pattern.SomeProperty;

            value.Should().Be(expectedValue);
        }

        [Fact]
        public void IsShouldForwardSubPropertyGetToStringLocalizer()
        {
            var expectedValue = "My Test Value";
            var expectedName = "My Test Name";

            mock.Setup(x => x[nameof(IMyObjectStringLocalizerPattern.MyObjectSubStringLocalizerProperty) + ":" + nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty)]).Returns(new LocalizedString(expectedName, expectedValue));

            var value = pattern.MyObjectSubStringLocalizerProperty.SomeSubProperty;

            value.Should().Be(expectedValue);
        }
    }
}
