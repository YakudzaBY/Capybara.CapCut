using System.Globalization;

namespace Capybara;

public class File
{
    public static async Task<TimeSpan[]> GetTimesAsync(string path)
    {
        return System.IO.File.Exists(path)
            ? [.. (await System.IO.File.ReadAllLinesAsync(path))
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .Select(str => TimeSpan.FromSeconds(double.Parse(str, CultureInfo.InvariantCulture)))]
            : [];
    }

    public static string GetFullPathWithoutExtension(string path)
    {
        return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
    }

    public static string GetCommonRoot(IEnumerable<string> paths, char? directorySeparator = default)
    {
        directorySeparator ??= Path.DirectorySeparatorChar;
        var parts = paths
            .Select(p => p.Split(directorySeparator.Value))
            .ToArray();

        var minLength = parts.Min(p => p.Length);
        var lastCommonIndex = -1;

        for (var i = 0; i < minLength; i++)
        {
            var part = default(string?);
            var result = true;
            foreach (var p in parts)
            {
                var thisPart = p[i];
                if (part == null) part = thisPart;
                else
                {
                    result = thisPart.Equals(part, StringComparison.OrdinalIgnoreCase);
                    if (!result) break;
                }
            }
            if (result) lastCommonIndex = i;
            else break;
        }

        return string.Join(directorySeparator.Value, parts.First(), 0, lastCommonIndex + 1);
    }

    public static bool ArePathsEquals(string path1, string path2)
    {
        return Path.GetFullPath(path1).Equals(Path.GetFullPath(path2), StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPathInside(string root, string child)
    {
        var normalizedRoot = Path.GetFullPath(root);
        var normalizedChild = Path.GetFullPath(child);
        return normalizedChild.StartsWith(normalizedRoot);
    }

    readonly static string[] reservedNames = [
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    ];

    readonly static char[] invalidPathChars = [.. Path.GetInvalidPathChars()
        .Concat(['<', '>', ':', '"', '/', '\\', '|', '?', '*'])
        .Distinct()];

    public static bool IsWindowsPathSupported(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        if (path.IndexOfAny(invalidPathChars) >= 0)
            return false;

        // Check for reserved device names
        string fileName = Path.GetFileName(path);

        if (reservedNames.Contains(fileName?.ToUpperInvariant()))
            return false;

        // Check for path length
        if (path.Length > 260)
            return false;

        return true;
    }
}
