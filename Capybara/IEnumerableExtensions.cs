namespace Capybara;

public static class IEnumerableExtensions
{
    public static T[,] To2dArray<T>(this T[][] source)
    {
        var maxCoulmns = source.Max(r => r.Length);

        var result = new T[source.Length, maxCoulmns];

        for (var r = 0; r < source.Length; r++)
        {
            var row = source[r];
            for (var c = 0; c < row.Length; c++)
            {
                result[r, c] = source[r][c];
            }
        }

        return result;
    }

    public static byte Sum(this IEnumerable<byte> source)
    {
        return source.Aggregate((a, b) => (byte)(a + b));
    }

    public static void ForEachFinalize<T>(this IEnumerable<T> source, Action<T> iterate, Action<T> finalize, T end)
    {
        foreach (var value in source)
        {
            finalize(value);
            iterate(value);
        }
        finalize(end);
    }

    public static bool HasAll<T>(this IEnumerable<T> source, IEnumerable<T>? values)
    {
        return values?.Any() != true
            || (source?.Any() == true && !values.Except(source).Any());
    }
}
