namespace JSLintNet.Console
{
    using System;

    public class Program
    {
        public static int Main(string[] args)
        {
            var provider = new ConsoleJSLintProvider();
            var factory = new ConsoleOptionsFactory();

            provider.WriteHeader();

            try
            {
                var options = factory.Create(args);

                if (options.Help)
                {
                    return provider.WriteHelp();
                }

                return provider.Lint(options);
            }
            catch (Exception ex)
            {
                return provider.WriteError(ex);
            }
        }
    }
}
