namespace Capybara;

public static class StringExtensions
{
    public static string Replace(this string input, IDictionary<string, string?> replacements)
    {
        foreach (var arg in replacements)
        {
            input = input.Replace($"{{{arg.Key}}}", arg.Value, StringComparison.OrdinalIgnoreCase);
        }
        return input;
    }


    public static string ToPaddedTable(this string[,] data, (string, bool)[] headers, string rowSeparator, string columnSeparator, out int[] columnWidths)
    {
        var rows = new List<string>();
        #region Calculate column widths
        columnWidths = new int[headers.Length];
        for (var c = 0; c < headers.Length; c++)
        {
            columnWidths[c] = headers[c].Item1.Length;
            for (var r = 0; r < data.GetLength(0); r++)
            {
                columnWidths[c] = Math.Max(columnWidths[c], data[r, c].Length);
            }
        }

        var localColumnWidths = columnWidths;
        string Pad(string cell, int columnNumber)
        {
            var width = localColumnWidths[columnNumber];
            Func<int, string> pad = headers[columnNumber].Item2 ? cell.PadLeft : cell.PadRight;
            //TODO account separator
            return pad(width);
        }
        #endregion

        #region Print headers
        rows.Add(string.Join(columnSeparator, headers.Select((h, i) => Pad(h.Item1, i))));
        #endregion

        #region Print rows
        for (var r = 0; r < data.GetLength(0); r++)
        {
            var cells = new List<string>();
            for (var c = 0; c < data.GetLength(1); c++)
            {
                cells.Add(Pad(data[r, c], c));
            }
            rows.Add(string.Join(columnSeparator, cells));
        }
        #endregion

        return string.Join(rowSeparator, rows);
    }

    private static readonly string[] pccNames = ["PCC", "PC"];

    public static string ToInitials(this string value)
    {
        return string.Join("", value
                .Replace("IPSC", null, StringComparison.OrdinalIgnoreCase)
                .Replace("Rifle", null, StringComparison.OrdinalIgnoreCase)
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(s => pccNames.Contains(s, StringComparer.OrdinalIgnoreCase)
                    ? s
                    : s[..1])
            );
    }
}
