// ----------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Helpers.Impl;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Helpers
{
    /// <summary>
    /// Helper static class to provide Json serialization methods.
    /// </summary>
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Default,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true
        };

        /// <summary>
        /// Deserialize Json data using provided options (or the one by default if null)
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="jsonSerializerOptions">The options to use with the serializer (may be null).</param>
        /// <returns></returns>
        public static async ValueTask<Dictionary<string, string>> DeserializeAsync(Stream stream, JsonSerializerOptions? jsonSerializerOptions)
        {
            var options = jsonSerializerOptions ?? DefaultJsonOptions;

            var jsonMapData = await JsonSerializer
                .DeserializeAsync<AJsonMapData>(stream, options)
                .ConfigureAwait(false);

            var map = new Dictionary<string, string>();
            jsonMapData.FillIn(string.Empty, map);

            return map;
        }
    }
}
