using System;
using Microsoft.Extensions.FileProviders;
using Moq;
using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using Xunit;

namespace SoloX.BlazorJsonLocalization.Tests.Core
{
    public class JsonStringLocalizerTest
    {
        [Fact]
        public void ItShouldSetupItPropertiesFromConstructor()
        {
            var fileProvider = Mock.Of<IFileProvider>();
            var resources = "Test";
            var name = "Name";

            var localizer = new JsonStringLocalizer(fileProvider, resources, name);

            Assert.Same(fileProvider, localizer.FileProvider);
            Assert.Same(resources, localizer.ResourcesPath);
            Assert.Same(name, localizer.Name);
        }
    }
}
