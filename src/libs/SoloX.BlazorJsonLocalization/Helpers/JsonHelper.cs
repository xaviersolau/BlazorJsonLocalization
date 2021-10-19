// ----------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

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
        /// <typeparam name="TData">The data type to load.</typeparam>
        /// <param name="stream">The input stream.</param>
        /// <param name="jsonSerializerOptions">The options to use with the serializer (may be null).</param>
        /// <returns></returns>
        public static ValueTask<TData> DeserializeAsync<TData>(Stream stream, JsonSerializerOptions? jsonSerializerOptions)
        {
            var options = jsonSerializerOptions ?? DefaultJsonOptions;

            return JsonSerializer
                .DeserializeAsync<TData>(stream, options);
        }
    }
}
