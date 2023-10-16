// ----------------------------------------------------------------------
// <copyright file="FileReaderTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using SoloX.BlazorJsonLocalization.Tools.Core.Impl;

namespace SoloX.BlazorJsonLocalization.Tools.Core.UTests
{
    public class FileReaderTest
    {
        [Fact]
        public void ItShouldReadTheFile()
        {
            var filePath = "./FileName.txt";

            var someText = "Some text...";

            File.WriteAllText(filePath, someText);

            var reader = new FileReader(".txt");

            string? text = null;

            reader.Read(".", "FileName", stream =>
            {
                using var textReader = new StreamReader(stream);

                text = textReader.ReadToEnd();
            });

            text.Should().NotBeNull().And.Be(someText);
        }
    }
}
