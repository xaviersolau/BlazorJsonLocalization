// ----------------------------------------------------------------------
// <copyright file="LocalizationMapConverter.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl.Localization
{
    /// <summary>
    /// Localization Map Json converter.
    /// </summary>
    public class LocalizationMapConverter : JsonConverter<ALocalizationData>
    {
        /// <inheritdoc/>
        public override ALocalizationData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var strValue = reader.GetString();

                return new LocalizationValue(strValue);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                if (!reader.Read())
                {
                    throw new JsonException();
                }

                return new LocalizationMap(ReadMap(ref reader, options));
            }
            else
            {
                throw new JsonException($"Unexpected token {reader.TokenType}");
            }
        }

        /// <inheritdoc/>
        private static Dictionary<string, ALocalizationData> ReadMap(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var map = new Dictionary<string, ALocalizationData>();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var key = reader.GetString();

                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }

                    map.Add(key, JsonSerializer.Deserialize<ALocalizationData>(ref reader, options));

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

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ALocalizationData value, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            switch (value)
            {
                case LocalizationMap map:
                    {
                        writer.WriteStartObject();
                        foreach (var item in map.ValueMap)
                        {
                            writer.WritePropertyName(item.Key);
                            Write(writer, item.Value, options);
                        }
                        writer.WriteEndObject();
                    }
                    break;
                case LocalizationValue mapValue:
                    {
                        writer.WriteStringValue(mapValue.Value);
                    }
                    break;
                default:
                    throw new JsonException();
            }
        }
    }
}
