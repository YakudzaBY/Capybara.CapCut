using System.Text.Json;

namespace Capybara.Json;

public class JsonPrefixedNamingPolicy(string Prefix, JsonNamingPolicy? subPolicy = default) : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        var result = subPolicy?.ConvertName(name) ?? name;
        return $"{Prefix}{result}";
    }
}
