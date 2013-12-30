namespace JSLintNet.Console
{
    internal interface IConsoleWriter
    {
        IConsoleWriter Write(string format, params object[] arg);

        IConsoleWriter Write(int indentLength, string format, params object[] arg);

        IConsoleWriter WriteLine();

        IConsoleWriter WriteLine(string format, params object[] arg);

        IConsoleWriter WriteLine(int indentLength, string format, params object[] arg);

        IConsoleWriter WriteError(string format, params object[] arg);

        IConsoleWriter WriteError(int indentLength, string format, params object[] arg);

        IConsoleWriter WriteErrorLine();

        IConsoleWriter WriteErrorLine(string format, params object[] arg);

        IConsoleWriter WriteErrorLine(int indentLength, string format, params object[] arg);
    }
}
