// ----------------------------------------------------------------------
// <copyright file="JsonHelperTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
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

            var map = await JsonHelper.DeserializeAsync(memoryStream, null).ConfigureAwait(false);

            map.Should().NotBeNull();
            map.Should().HaveCount(1);
            map.Should().ContainKey("test").WhoseValue.Should().Be("val1");
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

            var map = await JsonHelper.DeserializeAsync(memoryStream, null).ConfigureAwait(false);

            map.Should().NotBeNull();
            map.Should().HaveCount(1);
            map.Should().ContainKey("test").WhoseValue.Should().Be($"val1{separator}val2");
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

            var map = await JsonHelper.DeserializeAsync(memoryStream, null).ConfigureAwait(false);

            map.Should().NotBeNull();
            map.Should().HaveCount(3);
            map.Should().ContainKey("test1").WhoseValue.Should().Be("val1");
            map.Should().ContainKey("test2" + Key.Separator + "test21").WhoseValue.Should().Be("val21");
            map.Should().ContainKey("test2" + Key.Separator + "test22").WhoseValue.Should().Be("val22");
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

            var map = await JsonHelper.DeserializeAsync(memoryStream, null).ConfigureAwait(false);

            map.Should().NotBeNull();
            map.Should().HaveCount(2);
            map.Should().ContainKey("test1").WhoseValue.Should().Be("val11" + Environment.NewLine + "val12");
            map.Should().ContainKey("test2").WhoseValue.Should().Be("val21" + Environment.NewLine + "val22");
        }

        private static MemoryStream MakeJsonStream(string json)
        {
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            return memoryStream;
        }
    }
}
