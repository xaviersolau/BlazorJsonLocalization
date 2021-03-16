// ----------------------------------------------------------------------
// <copyright file="ExtensionResolverServiceTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Moq;
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
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(p => p.GetService(typeof(IJsonLocalizationExtensionService<MyOptions>)))
                .Returns(new MyExtensionService());

            var extensionOptionsContainer = new ExtensionOptionsContainer<MyOptions>(new MyOptions());

            var extensionResolverService = new ExtensionResolverService(serviceProviderMock.Object);
            var extensionService = extensionResolverService.GetExtensionService(extensionOptionsContainer);

            Assert.NotNull(extensionService);

            serviceProviderMock.Verify(p => p.GetService(typeof(IJsonLocalizationExtensionService<MyOptions>)));
            serviceProviderMock.VerifyNoOtherCalls();
        }
    }
}
