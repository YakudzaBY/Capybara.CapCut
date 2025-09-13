using System.Text.Json.Serialization;
using System.Text.Json;

namespace Capybara.CapCut.Json.Converters;

public class PropperCaseGuidJsonConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Guid.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        const char separator = '-';
        var parts = value.ToString().Split(separator).Select((s, i) => i == 2 ? s.ToLower() : s.ToUpper());
        writer.WriteStringValue(string.Join(separator, parts));
        //writer.WriteStringValue(value.ToString().ToLower());
    }
}
