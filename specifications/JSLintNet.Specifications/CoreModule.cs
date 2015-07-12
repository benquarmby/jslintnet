namespace JSLintNet.Specifications
{
    using System;
    using System.Reflection;
    using Autofac;
    using DependencyModule = Autofac.Module;

    public class CoreModule : DependencyModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(typeof(JSLint).Assembly)
                .Where(IsNotAbstraction)
                .AsImplementedInterfaces()
                .AsSelf()
                .FindConstructorsWith(x =>
                {
                    return x.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                });
        }

        private static bool IsNotAbstraction(Type type)
        {
            return !type.Namespace.Contains("Abstraction");
        }
    }
}
