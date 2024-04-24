// ----------------------------------------------------------------------
// <copyright file="JsonMapDataConverter.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoloX.BlazorJsonLocalization.Helpers.Impl
{
#pragma warning disable CA1812
    internal class JsonMapDataConverter : JsonConverter<AJsonMapData>
#pragma warning restore CA1812
    {
        public override AJsonMapData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var strValue = reader.GetString();

                return new JsonMapDataValue { Value = strValue };
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                if (!reader.Read())
                {
                    throw new JsonException();
                }

                return new JsonMapDataValue { Value = JsonMapDataConverter.ReadStringArray(ref reader) };
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                if (!reader.Read())
                {
                    throw new JsonException();
                }

                return new JsonMapDataDictionary { ValueMap = ReadMap(ref reader, options) };
            }
            else
            {
                throw new JsonException($"Unexpected token {reader.TokenType}");
            }
        }

        private static string ReadStringArray(ref Utf8JsonReader reader)
        {
            var stringBuilder = new StringBuilder();
            var first = true;

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var strValue = reader.GetString();

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        stringBuilder.AppendLine();
                    }

                    stringBuilder.Append(strValue);

                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }
                }
                else
                {
                    throw new JsonException($"Unexpected token {reader.TokenType} only string are supported in Json array");
                }
            }

            return stringBuilder.ToString();
        }

        private static Dictionary<string, AJsonMapData> ReadMap(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var map = new Dictionary<string, AJsonMapData>();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var key = reader.GetString();

                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }

                    map.Add(key, JsonSerializer.Deserialize<AJsonMapData>(ref reader, options));

                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }
                }
                else
                {
                    throw new JsonException();
                }
            }

            return map;
        }

        public override void Write(Utf8JsonWriter writer, AJsonMapData value, JsonSerializerOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}
