// ----------------------------------------------------------------------
// <copyright file="AJsonMapData.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SoloX.BlazorJsonLocalization.Helpers.Impl
{
    [JsonConverter(typeof(JsonMapDataConverter))]
    internal abstract class AJsonMapData
    {
        public abstract void FillIn(string? prefix, Dictionary<string, string> map);
    }
}
