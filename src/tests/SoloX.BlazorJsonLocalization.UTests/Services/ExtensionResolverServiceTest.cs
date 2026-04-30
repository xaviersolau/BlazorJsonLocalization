// ----------------------------------------------------------------------
// <copyright file="ExtensionResolverServiceTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using NSubstitute;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.Services.Impl;
using SoloX.BlazorJsonLocalization.UTests.Samples.Extension;
using System;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Services
{
    public class ExtensionResolverServiceTest
    {
        [Fact]
        public void ItShouldGetTheAppropriateExtensionService()
        {
            var serviceProviderMock = Substitute.For<IServiceProvider>();

            serviceProviderMock
                .GetService(typeof(IJsonLocalizationExtensionService<MyOptions>))
                .Returns(new MyExtensionService());

            var extensionOptionsContainer = new ExtensionOptionsContainer<MyOptions>(new MyOptions());

            var extensionResolverService = new ExtensionResolverService(serviceProviderMock);
            var extensionService = extensionResolverService.GetExtensionService(extensionOptionsContainer);

            Assert.NotNull(extensionService);

            serviceProviderMock.Received().GetService(typeof(IJsonLocalizationExtensionService<MyOptions>));
        }
    }
}
