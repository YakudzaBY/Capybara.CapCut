using System.Text.Json;
using System.Text.Json.Serialization;

namespace Capybara.Json.Converters;

public class JsonStringByteConverter : JsonConverter<byte>
{
    public override byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return byte.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, byte value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
