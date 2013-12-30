namespace JSLintNet.Console.Abstractions
{
    internal interface IConsoleWrapper
    {
        int BufferHeight { get; }

        int BufferWidth { get; }

        IConsoleWrapper Write(string format, params object[] arg);

        IConsoleWrapper WriteLine();

        IConsoleWrapper WriteLine(string format, params object[] arg);

        IConsoleWrapper WriteError(string format, params object[] arg);

        IConsoleWrapper WriteErrorLine();

        IConsoleWrapper WriteErrorLine(string format, params object[] arg);
    }
}
