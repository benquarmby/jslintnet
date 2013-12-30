namespace JSLintNet.QualityTools.Helpers
{
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public static class ProcessHelper
    {
        public static ProcessResult Execute(string exePath, string arguments)
        {
            return Execute(exePath, arguments, null);
        }

        public static ProcessResult Execute(string exePath, string arguments, string workingDirectory)
        {
            int exitCode;
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = exePath,
                    WorkingDirectory = string.IsNullOrEmpty(workingDirectory) ? Path.GetDirectoryName(exePath) : workingDirectory,
                    Arguments = arguments
                };

                process.OutputDataReceived += (s, e) => outputBuilder.AppendLine(e.Data);
                process.ErrorDataReceived += (s, e) => errorBuilder.AppendLine(e.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                exitCode = process.ExitCode;
            }

            return new ProcessResult()
            {
                ExitCode = exitCode,
                Output = outputBuilder.ToString(),
                Error = errorBuilder.ToString()
            };
        }
    }
}
