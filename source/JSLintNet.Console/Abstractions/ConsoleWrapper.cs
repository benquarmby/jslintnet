namespace JSLintNet.Console.Abstractions
{
    using System;

    internal class ConsoleWrapper : IConsoleWrapper
    {
        public int BufferHeight
        {
            get
            {
                if (Console.IsOutputRedirected && Console.IsErrorRedirected)
                {
                    return 80;
                }

                return Console.BufferHeight;
            }
        }

        public int BufferWidth
        {
            get
            {
                if (Console.IsOutputRedirected && Console.IsErrorRedirected)
                {
                    return 80;
                }

                return Console.BufferWidth;
            }
        }

        public IConsoleWrapper Write(string format, params object[] arg)
        {
            Console.Write(format, arg);

            return this;
        }

        public IConsoleWrapper WriteLine()
        {
            Console.WriteLine();

            return this;
        }

        public IConsoleWrapper WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);

            return this;
        }

        public IConsoleWrapper WriteError(string format, params object[] arg)
        {
            Console.Error.Write(format, arg);

            return this;
        }

        public IConsoleWrapper WriteErrorLine()
        {
            Console.Error.WriteLine();

            return this;
        }

        public IConsoleWrapper WriteErrorLine(string format, params object[] arg)
        {
            Console.Error.WriteLine(format, arg);

            return this;
        }
    }
}
