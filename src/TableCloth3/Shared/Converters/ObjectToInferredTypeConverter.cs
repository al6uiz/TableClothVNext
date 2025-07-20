using System.Text.Json;
using System.Text.Json.Serialization;

namespace TableCloth3.Shared.Converters;

public sealed class ObjectToInferredTypeConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonElementToValue(JsonDocument.ParseValue(ref reader).RootElement, options);
    }

    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value?.GetType() ?? typeof(object), options);
    }

    private static object? JsonElementToValue(JsonElement element, JsonSerializerOptions options)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.Deserialize<Dictionary<string, object?>>(options),
            JsonValueKind.Array => element.Deserialize<List<object?>>(options),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var l) ? l :
                                    element.TryGetDouble(out var d) ? d : (object?)null,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => null
        };
    }
}
