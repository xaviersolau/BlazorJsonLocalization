// ----------------------------------------------------------------------
// <copyright file="SatelliteFileProviderFactoryTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Shouldly;
using SoloX.BlazorJsonLocalization.Core.Impl;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class SatelliteFileProviderFactoryTest
    {
        [Theory]
        [InlineData("fr", @"Resources/SatelliteFileProvider/Test.fr.json", "TestValueFromFr")]
        [InlineData("fr", @"Resources/SatelliteFileProvider/Test.json", "TestValue")]
        [InlineData("en", @"Resources/SatelliteFileProvider/Test.json", "TestValue")]
        public void ItShouldLoadResouvesFromSatelliteAssembly(string cultureName, string filePath, string exprectedValue)
        {
            var ci = CultureInfo.GetCultureInfo(cultureName);

            var factory = new SatelliteFileProviderFactory(
                typeof(SatelliteFileProviderFactoryTest).Assembly,
                "SoloX.BlazorJsonLocalization.UTests");

            var fileProvider = factory.GetFileProvider(ci);

            var file = fileProvider.GetFileInfo(filePath);

            file.Exists.ShouldBeTrue();

            var map = JsonSerializer.Deserialize<IDictionary<string, string>>(file.CreateReadStream());

            map.ShouldNotBeNull();

            map["Test"].ShouldBe(exprectedValue);
        }
    }
}
