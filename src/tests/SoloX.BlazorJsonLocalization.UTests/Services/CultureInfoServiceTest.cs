// ----------------------------------------------------------------------
// <copyright file="CultureInfoServiceTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Services.Impl;
using SoloX.CodeQuality.Test.Helpers.XUnit.Logger;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.UTests.Services
{
    public class CultureInfoServiceTest
    {
        private ILogger<CultureInfoService> Logger { get; }

        public CultureInfoServiceTest(ITestOutputHelper testOutputHelper)
        {
            Logger = new TestLogger<CultureInfoService>(testOutputHelper);
        }

        [Fact]
        public void IsShouldGetCultureInfoFromStaticCurrentUICulture()
        {
            var service = new CultureInfoService(Logger);

            Assert.Equal(CultureInfo.CurrentUICulture, service.CurrentUICulture);
        }
    }
}
