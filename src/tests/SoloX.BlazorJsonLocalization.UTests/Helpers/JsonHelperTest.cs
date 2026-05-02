// ----------------------------------------------------------------------
// <copyright file="JsonHelperTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Shouldly;
using SoloX.BlazorJsonLocalization.Helpers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoloX.BlazorJsonLocalization.UTests.Helpers
{
    public class JsonHelperTest
    {
        [Fact]
        public async Task ItShouldReadJsonResources()
        {
            var json = @"
            {
                ""test"" : ""val1""
            }
            ";
            using var memoryStream = MakeJsonStream(json);

            var map = await JsonHelper.DeserializeAsync(memoryStream, null);

            map.ShouldNotBeNull();
            map.ShouldHaveSingleItem();
            map.ShouldContainKeyAndValue("test", "val1");
        }

        [Theory]
        [InlineData("val1\\r\\nval2", "\r\n")]
        [InlineData("val1\\nval2", "\n")]
        public async Task ItShouldReadJsonValueWithNewLineSeparator(string jsonValue, string separator)
        {
            var json = $@"
            {{
                ""test"" : ""{jsonValue}""
            }}
            ";
            using var memoryStream = MakeJsonStream(json);

            var map = await JsonHelper.DeserializeAsync(memoryStream, null);

            map.ShouldNotBeNull();
            map.ShouldHaveSingleItem();
            map.ShouldContainKeyAndValue("test", $"val1{separator}val2");
        }

        [Fact]
        public async Task ItShouldReadJsonStructuredResources()
        {
            var json = @"
            {
                ""test1"" : ""val1"",
                ""test2"" : {
                    ""test21"" : ""val21"",
                    ""test22"" : ""val22""
                }
            }
            ";

            using var memoryStream = MakeJsonStream(json);

            var map = await JsonHelper.DeserializeAsync(memoryStream, null);

            map.ShouldNotBeNull();
            map.Count.ShouldBe(3);

            map.ShouldContainKeyAndValue("test1", "val1");
            map.ShouldContainKeyAndValue("test2" + Key.Separator + "test21", "val21");
            map.ShouldContainKeyAndValue("test2" + Key.Separator + "test22", "val22");
        }

        [Fact]
        public async Task ItShouldReadMultiLineJsonResources()
        {
            var json = @"
            {
                ""test1"" : [""val11"", ""val12""],
                ""test2"" : [""val21"", ""val22""]
            }
            ";
            using var memoryStream = MakeJsonStream(json);

            var map = await JsonHelper.DeserializeAsync(memoryStream, null);

            map.ShouldNotBeNull();
            map.Count.ShouldBe(2);
            map.ShouldContainKeyAndValue("test1", "val11" + Environment.NewLine + "val12");
            map.ShouldContainKeyAndValue("test2", "val21" + Environment.NewLine + "val22");
        }

        private static MemoryStream MakeJsonStream(string json)
        {
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            return memoryStream;
        }
    }
}
