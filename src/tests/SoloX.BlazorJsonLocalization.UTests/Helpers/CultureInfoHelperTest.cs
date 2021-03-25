// ----------------------------------------------------------------------
// <copyright file="CultureInfoHelperTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Helpers
{
    public class CultureInfoHelperTest
    {
        [Fact]
        public async Task ItShouldCallTheDataLoaderWithTheCultureInfoNameAsync()
        {
            var cultureName = "fr-FR";

            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            var data = await CultureInfoHelper.WalkThoughCultureInfoParentsAsync<string>(
                cultureInfo,
                name =>
                {
                    return ValueTask.FromResult<string?>(name);
                }).ConfigureAwait(false);

            Assert.Equal(cultureName, data);
        }

        [Fact]
        public async Task ItShouldWalkThroughTheCultureInfoParentAsync()
        {
            var cultureName = "fr-FR";

            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            var list = new List<string>();

            var data = await CultureInfoHelper.WalkThoughCultureInfoParentsAsync<string>(
                cultureInfo,
                name =>
                {
                    list.Add(name);
                    return ValueTask.FromResult<string?>(null);
                }).ConfigureAwait(false);

            Assert.Null(data);

            Assert.Equal(3, list.Count);
            Assert.Equal(cultureInfo.Name, list[0]);
            Assert.Equal(cultureInfo.Parent.Name, list[1]);
            Assert.Equal(cultureInfo.Parent.Parent.Name, list[2]);
        }
    }
}
