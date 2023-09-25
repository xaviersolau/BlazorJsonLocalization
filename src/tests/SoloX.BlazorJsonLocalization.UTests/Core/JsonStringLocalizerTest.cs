// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using Moq;
using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Core
{
    public class JsonStringLocalizerTest
    {
        [Theory]
        [InlineData("TestKey", "TestText", "TestKey", null, "TestText", false)]
        [InlineData("TestKey", "TestText", "BadKey", null, "BadKey", true)]
        [InlineData("TestKey", "TestText_{0}", "TestKey", "MyArg", "TestText_MyArg", false)]
        [InlineData("TestKey", "TestText_{0}", "BadKey", "MyArg", "BadKey", true)]
        public void ItShouldBuildLocalizedStringFromGivenMap(
            string resourceKey, string resourceText,
            string queryKey, string queryArg,
            string expectedText, bool expectedNotFound)
        {
            var localizer = CreateLocalizer(resourceKey, resourceText);

            var localized = string.IsNullOrEmpty(queryArg)
                ? localizer[queryKey]
                : localizer[queryKey, queryArg];

            Assert.Same(queryKey, localized.Name);

            Assert.Equal(expectedText, localized.Value);
            Assert.Equal(expectedNotFound, localized.ResourceNotFound);
        }

        private static JsonStringLocalizer CreateLocalizer(string resourceKey, string resourceText, string cultureName = "en-us")
        {
            var localizerParentCultureLocalizerMock = new Mock<IStringLocalizerInternal>();

            var localizerFactoryMock = new Mock<IJsonStringLocalizerFactoryInternal>();
            localizerFactoryMock.Setup(x => x.CreateStringLocalizer(It.IsAny<CultureInfo>())).Returns(localizerParentCultureLocalizerMock.Object);

            var map = new Dictionary<string, string>
            {
                [resourceKey] = resourceText,
            };

            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            var localizer = new JsonStringLocalizer(map, cultureInfo, localizerFactoryMock.Object);
            return localizer;
        }
    }
}
