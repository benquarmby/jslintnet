namespace JSLintNet.Console
{
    using System;

    public class Program
    {
        [STAThread]
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

                if (options.SettingsEditor)
                {
                    return provider.EditSettings(options);
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
