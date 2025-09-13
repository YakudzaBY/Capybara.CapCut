using System.Text.Json;
using System.Text.Json.Serialization;

namespace Capybara.Json.Converters;

public class JsonEncodedStringToObjectConverter<T> : JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(reader.GetString(), options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var str = JsonSerializer.Serialize(value, options);
        writer.WriteStringValue(str);
    }
}
