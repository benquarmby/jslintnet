namespace JSLintNet.Console
{
    using System;
    using System.Text;
    using JSLintNet.Console.Abstractions;

    internal class ConsoleWriter : IConsoleWriter
    {
        private IConsoleWrapper consoleWrapper;

        public ConsoleWriter()
            : this(new ConsoleWrapper())
        {
        }

        public ConsoleWriter(IConsoleWrapper consoleWrapper)
        {
            this.consoleWrapper = consoleWrapper;
        }

        public IConsoleWriter Write(string format, params object[] arg)
        {
            this.Write(0, format, arg);

            return this;
        }

        public IConsoleWriter Write(int indentLength, string format, params object[] arg)
        {
            var value = this.BreakLines(indentLength, format, arg);
            this.consoleWrapper.Write(Escape(value));

            return this;
        }

        public IConsoleWriter WriteLine()
        {
            this.consoleWrapper.WriteLine();

            return this;
        }

        public IConsoleWriter WriteLine(string format, params object[] arg)
        {
            this.WriteLine(0, format, arg);

            return this;
        }

        public IConsoleWriter WriteLine(int indentLength, string format, params object[] arg)
        {
            this.Write(indentLength, format, arg);
            this.consoleWrapper.WriteLine();

            return this;
        }

        public IConsoleWriter WriteError(string format, params object[] arg)
        {
            this.WriteError(0, format, arg);

            return this;
        }

        public IConsoleWriter WriteError(int indentLength, string format, params object[] arg)
        {
            var value = this.BreakLines(indentLength, format, arg);
            this.consoleWrapper.WriteError(Escape(value));

            return this;
        }

        public IConsoleWriter WriteErrorLine()
        {
            this.consoleWrapper.WriteErrorLine();

            return this;
        }

        public IConsoleWriter WriteErrorLine(string format, params object[] arg)
        {
            this.WriteErrorLine(0, format, arg);

            return this;
        }

        public IConsoleWriter WriteErrorLine(int indentLength, string format, params object[] arg)
        {
            this.WriteError(indentLength, format, arg);
            this.consoleWrapper.WriteErrorLine();

            return this;
        }

        private static string Escape(string value)
        {
            return value.Replace("{", "{{").Replace("}", "}}");
        }

        private string BreakLines(int indentLength, string format, params object[] arg)
        {
            if (indentLength > 0 && this.consoleWrapper.BufferWidth / indentLength < 2)
            {
                throw new ArgumentException("The indent must be less than half the maximum line length.", "indentLength");
            }

            var value = string.Format(format, arg);
            var words = value.Split(' ');
            var lineBuilder = new StringBuilder();
            var paragraphBuilder = new StringBuilder();
            var maxLength = this.consoleWrapper.BufferWidth - indentLength - 1;
            var indent = new string(' ', indentLength);

            foreach (var word in words)
            {
                if (lineBuilder.Length > 0 && lineBuilder.Length + word.Length >= maxLength)
                {
                    paragraphBuilder
                        .Append(indent)
                        .AppendLine(lineBuilder.ToString());

                    lineBuilder.Clear();
                }

                if (lineBuilder.Length > 0)
                {
                    lineBuilder.Append(' ');
                }

                lineBuilder.Append(word);
            }

            paragraphBuilder
                .Append(indent)
                .Append(lineBuilder.ToString());

            return paragraphBuilder.ToString();
        }
    }
}
