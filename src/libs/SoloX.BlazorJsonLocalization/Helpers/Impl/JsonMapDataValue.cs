// ----------------------------------------------------------------------
// <copyright file="JsonMapDataValue.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Collections.Generic;

namespace SoloX.BlazorJsonLocalization.Helpers.Impl
{
    internal class JsonMapDataValue : AJsonMapData
    {
        public string Value { get; set; }

        public override void FillIn(string? prefix, Dictionary<string, string> map)
        {
            map.Add(prefix, Value);
        }
    }
}
