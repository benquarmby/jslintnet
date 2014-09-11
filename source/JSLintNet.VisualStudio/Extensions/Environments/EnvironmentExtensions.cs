namespace EnvDTE80
{
    using System;
    using JSLintNet.VisualStudio.Extensions.Environments;
    using Environment = EnvDTE80.DTE2;

    internal static class EnvironmentExtensions
    {
        static EnvironmentExtensions()
        {
            LocatorFactory = x => new EnvironmentLocator(x);
        }

        internal static Func<Environment, IEnvironmentLocator> LocatorFactory { get; set; }

        public static IEnvironmentLocator Locate(this Environment environment)
        {
            return LocatorFactory(environment);
        }
    }
}
