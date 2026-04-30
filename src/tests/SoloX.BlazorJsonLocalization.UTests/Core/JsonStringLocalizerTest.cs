// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using NSubstitute;
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
            string queryKey, string? queryArg,
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
            var resourceSource = new StringLocalizerResourceSource("test", typeof(ConstStringLocalizerTest).Assembly, null);

            var localizerParentCultureLocalizerMock = Substitute.For<IStringLocalizerInternal>();

            var localizerFactoryInternalMock = Substitute.For<IJsonStringLocalizerFactoryInternal>();

            localizerFactoryInternalMock.CreateStringLocalizer(Arg.Any<StringLocalizerResourceSource>(), Arg.Any<CultureInfo>()).Returns(localizerParentCultureLocalizerMock);

            localizerFactoryInternalMock
                .FindThroughStringLocalizerHierarchy(Arg.Any<IStringLocalizerInternal>(), Arg.Any<CultureInfo>(), Arg.Any<Func<IStringLocalizerInternal, LocalizedString?>>())
                .Returns(ci =>
                {
                    var localizer = ci.Arg<IStringLocalizerInternal>();
                    var forward = ci.Arg<Func<IStringLocalizerInternal, LocalizedString?>>();
                    return forward(localizer);
                });

            localizerFactoryInternalMock
                .CreateDefaultStringLocalizer(resourceSource, Arg.Any<CultureInfo>(), Arg.Any<string>())
                .Returns(ci =>
                {
                    // <StringLocalizerResourceSource, CultureInfo, string>
                    var source = ci.Arg<StringLocalizerResourceSource>();
                    var culture = ci.Arg<CultureInfo>();
                    var id = ci.Arg<string>();
                    return new NullStringLocalizer(source, culture, localizerFactoryInternalMock, true, id);
                });

            var map = new Dictionary<string, string>
            {
                [resourceKey] = resourceText,
            };

            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            var localizer = new JsonStringLocalizer(resourceSource, map, cultureInfo, localizerFactoryInternalMock);
            return localizer;
        }
    }
}
