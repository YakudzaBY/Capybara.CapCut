using System.Diagnostics;

namespace Capybara;

public class Cmd
{
    public static async Task<string> RunAsync(string args)
    {
        var p = Process.Start(new ProcessStartInfo
        {
            FileName = "CMD.EXE",
            Arguments = $"/C {args}",
            //UseShellExecute = false,
            RedirectStandardOutput = true
        })!;

        var result = await p.StandardOutput.ReadToEndAsync();
        await p.WaitForExitAsync();
        return result;
    }
}
