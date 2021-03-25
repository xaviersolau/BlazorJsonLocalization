// ----------------------------------------------------------------------
// <copyright file="ResourcePathHelperTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Helpers;
using System;
using System.Reflection;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Helpers
{
    public class ResourcePathHelperTest
    {
        private static readonly string BaseName = typeof(ResourcePathHelperTest).FullName;

        private static readonly Assembly Assembly = typeof(ResourcePathHelperTest).Assembly;

        [Fact]
        public void ItShouldComputeTheResourcePath()
        {
            var path = ResourcePathHelper.ComputeBasePath(Assembly, BaseName);

            var expectedPath = BaseName
                .Replace(Assembly.GetName().Name + ".", string.Empty, StringComparison.Ordinal)
                .Replace('.', '/');

            Assert.Equal(expectedPath, path);
        }
    }
}
