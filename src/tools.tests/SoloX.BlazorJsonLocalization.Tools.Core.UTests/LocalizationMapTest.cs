using FluentAssertions;
using SoloX.BlazorJsonLocalization.Tools.Core.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoloX.BlazorJsonLocalization.Tools.Core.UTests
{
    public class LocalizationMapTest
    {
        private const string Target = @"
        {
          ""Property1"": ""Property1"",
          ""Property2"": {
            ""SubProperty1"": ""SubProperty1"",
            ""SubProperty2"": ""SubProperty2""
          }
        }";

        private const string Source1 = @"
        {
          ""Property1"": ""SourceProperty1"",
          ""Property2"": {
            ""SubProperty1"": ""SourceSubProperty1"",
            ""SubProperty2"": ""SourceSubProperty2""
          }
        }";

        private const string Expected1 = Source1;

        private const string Source2 = @"
        {
          ""Property1"": ""SourceProperty1""
        }";

        private const string Expected2 = @"
        {
          ""Property1"": ""SourceProperty1"",
          ""Property2"": {
            ""SubProperty1"": ""SubProperty1"",
            ""SubProperty2"": ""SubProperty2""
          }
        }";

        private const string Source3 = @"
        {
          ""Property1"": ""SourceProperty1"",
          ""Property3"": ""SourceProperty3""
        }";

        private const string Expected3 = @"
        {
          ""Property1"": ""SourceProperty1"",
          ""Property2"": {
            ""SubProperty1"": ""SubProperty1"",
            ""SubProperty2"": ""SubProperty2""
          },
          ""Property3"": ""SourceProperty3""
        }";

        private const string Source4 = @"
        {
          ""Property2"": {
            ""SubProperty1"": ""SourceSubProperty1"",
            ""SubProperty2"": ""SourceSubProperty2"",
            ""SubProperty3"": ""SourceSubProperty3""
          }
        }";

        private const string Expected4 = @"
        {
          ""Property1"": ""Property1"",
          ""Property2"": {
            ""SubProperty1"": ""SourceSubProperty1"",
            ""SubProperty2"": ""SourceSubProperty2"",
            ""SubProperty3"": ""SourceSubProperty3""
          }
        }";

        private const string Source5 = @"
        {
          ""Property2"": ""SourceProperty1"",
          ""Property1"": {
            ""SubProperty1"": ""SourceSubProperty1"",
            ""SubProperty2"": ""SourceSubProperty2""
          }
        }";

        private const string Expected5 = Target;

        private const string Source6 = @"
        {
          ""Property2"": {
            ""SubProperty1"": ""SourceSubProperty1"",
            ""SubProperty2"": ""SourceSubProperty2""
          },
          ""Property1"": ""SourceProperty1""
        }";

        private const string Expected6 = @"
        {
          ""Property1"": ""SourceProperty1"",
          ""Property2"": {
            ""SubProperty1"": ""SourceSubProperty1"",
            ""SubProperty2"": ""SourceSubProperty2""
          }
        }";

        [Theory]
        [InlineData(Target, Target, Target, false)]
        [InlineData(Source1, Target, Expected1, false)]
        [InlineData(Source2, Target, Expected2, true)]
        [InlineData(Source3, Target, Expected3, true)]
        [InlineData(Source4, Target, Expected4, true)]
        [InlineData(Source5, Target, Expected5, true)]
        [InlineData(Source6, Target, Expected6, false)]
        public void ItShouldMergeSourceWithTarget(string source, string target, string expected, bool expectedDirty)
        {
            expected = expected.Replace("        ", string.Empty).Trim();

            var sourceMap = JsonSerializer.Deserialize<ALocalizationData>(source);
            var targetMap = JsonSerializer.Deserialize<ALocalizationData>(target);

            var mergedMap = targetMap.Merge(sourceMap, string.Empty, out var dirty);

            dirty.Should().Be(expectedDirty);

            mergedMap.Should().NotBeNull();

            var output = JsonSerializer.Serialize(mergedMap, new JsonSerializerOptions { WriteIndented = true });

            output.Should().Be(expected);
        }
    }
}
