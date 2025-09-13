namespace Capybara;

public static class DictionaryExtensions
{
    public static TValue GetOrCreate<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> d, TKey key, Func<TKey, TValue> create)
    {
        if(!d.TryGetValue(key, out var value))
        {
            value = create(key);
        }
        return value;
    }

    public static TValue? GetOptionalOrCreate<TValue>(this IReadOnlyDictionary<string, TValue> d, string key, Func<string, TValue> create)
    {
        return string.IsNullOrWhiteSpace(key)
            ? default
            : d.GetOrCreate(key, create);
    }

    public static TValue? TryGetValue<TValue>(this IReadOnlyDictionary<string, TValue> d, string key)
    {
        return d
            .Where(kvp => kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
            .Select(kvp => kvp.Value)
            .SingleOrDefault();
    }
}
