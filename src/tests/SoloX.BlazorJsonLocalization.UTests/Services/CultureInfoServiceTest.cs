// ----------------------------------------------------------------------
// <copyright file="CultureInfoServiceTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Services.Impl;
using System.Globalization;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Services
{
    public class CultureInfoServiceTest
    {
        [Fact]
        public void IsShouldGetCultureInfoFromStaticCurrentUICulture()
        {
            var service = new CultureInfoService();

            Assert.Equal(CultureInfo.CurrentUICulture, service.CurrentUICulture);
        }
    }
}
