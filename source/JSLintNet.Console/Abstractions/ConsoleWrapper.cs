namespace JSLintNet.Console.Abstractions
{
    using System;

    internal class ConsoleWrapper : IConsoleWrapper
    {
        public int BufferHeight
        {
            get
            {
                try
                {
                    return Console.BufferHeight;
                }
                catch
                {
                    // If both standard output and standard error are redirected, there is no buffer and the getter will throw.
                    // After upgrading to .NET 4.5, this can be replaced by checking IsOutputRedirected and IsErrorRedirected.
                    return 80;
                }
            }
        }

        public int BufferWidth
        {
            get
            {
                try
                {
                    return Console.BufferWidth;
                }
                catch
                {
                    // If both standard output and standard error are redirected, there is no buffer and the getter will throw.
                    // After upgrading to .NET 4.5, this can be replaced by checking IsOutputRedirected and IsErrorRedirected.
                    return 80;
                }
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
