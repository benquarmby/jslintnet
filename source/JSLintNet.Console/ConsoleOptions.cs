namespace JSLintNet.Console
{
    using System.Collections.Generic;

    internal enum LogLevel
    {
        Silent = -1,
        Normal = 0,
        Verbose = 1
    }

    internal class ConsoleOptions
    {
        public string SourceDirectory { get; set; }

        public string ReportFile { get; set; }

        public string SettingsFile { get; set; }

        public LogLevel LogLevel { get; set; }

        public bool Help { get; set; }

        public bool SettingsEditor { get; set; }

        public JSLintNetSettings Settings { get; set; }

        public IList<string> SourceFiles { get; set; }
    }
}
