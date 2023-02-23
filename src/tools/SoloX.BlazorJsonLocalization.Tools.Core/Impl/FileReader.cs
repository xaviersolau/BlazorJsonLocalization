// ----------------------------------------------------------------------
// <copyright file="FileReader.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.IO;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    /// <summary>
    /// File Reader implementation.
    /// </summary>
    public class FileReader : IReader
    {
        private readonly string fileSufix;

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="fileSufix">File suffix to use to build the file path.</param>
        public FileReader(string fileSufix)
        {
            this.fileSufix = fileSufix;
        }

        /// <inheritdoc/>
        public void Read(string location, string name, Action<Stream> reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var path = name + this.fileSufix;

            var text = Path.Combine(location, path);

            if (File.Exists(text))
            {
                using (var stream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    reader(stream);
                }
            }
        }
    }
}
