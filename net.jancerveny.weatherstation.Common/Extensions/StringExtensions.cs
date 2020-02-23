using System.Diagnostics;
using System.Threading.Tasks;

public static class StringExtensions
{
    public static async Task<string> BashAsync(this string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");
        var tcs = new TaskCompletionSource<string>();

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Exited += (sender, args) =>
        {
            tcs.SetResult(process.StandardOutput.ReadToEnd());
            process.Dispose();
        };
        process.Start();
        //string result = process.StandardOutput.ReadToEnd();
        //process.WaitForExit();
        //return result;
        return await tcs.Task;
    }

    public static string Bash(this string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;
    }
}

