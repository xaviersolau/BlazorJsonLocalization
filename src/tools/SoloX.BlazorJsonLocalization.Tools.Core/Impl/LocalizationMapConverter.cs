using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    public class LocalizationMapConverter : JsonConverter<ALocalizationData>
    {
        public override ALocalizationData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var strValue = reader.GetString();

                return new LocalizationValue { Value = strValue };
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                if (!reader.Read())
                {
                    throw new JsonException();
                }

                return new LocalizationMap { ValueMap = ReadMap(ref reader, options) };
            }
            else
            {
                throw new JsonException($"Unexpected token {reader.TokenType}");
            }
        }

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

        public override void Write(Utf8JsonWriter writer, ALocalizationData value, JsonSerializerOptions options)
        {
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
