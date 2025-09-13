namespace Capybara.CapCut;

public static class StringExtensions
{
    public static int CountLines(this string str)
    {
        return str.Count(c => c == Constants.NewLine) + 1;
    }
}
