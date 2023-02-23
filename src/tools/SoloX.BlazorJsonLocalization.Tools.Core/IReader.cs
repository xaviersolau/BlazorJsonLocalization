// ----------------------------------------------------------------------
// <copyright file="IReader.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.IO;

namespace SoloX.BlazorJsonLocalization.Tools.Core
{
    /// <summary>
    /// Stream Reader interface.
    /// </summary>
    public interface IReader
    {
        /// <summary>
        /// Read a Stream at the given location and with the given name.
        /// If the resource is not found, the reader delegate won't be called.
        /// </summary>
        /// <param name="location">Location of the stream to read.</param>
        /// <param name="name">Name of the resource.</param>
        /// <param name="reader">Reader delegate.</param>
        void Read(string location, string name, Action<Stream> reader);
    }
}
