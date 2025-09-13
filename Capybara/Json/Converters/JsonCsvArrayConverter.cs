using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Capybara.Json.Converters
{
    public class JsonCsvArrayConverter : JsonConverter<string[]>
    {
        public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
            var strings = reader.GetString().Split(',')
                .Select(s => TimeSpan.ParseExact(s, @"ss.ff", CultureInfo.InvariantCulture));
        }

        public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
