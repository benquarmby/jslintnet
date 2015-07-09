namespace JSLintNet.QualityTools
{
    using System;
    using System.Reflection;
    using Autofac;

    public class AutofacTestFixture<T> : TestFixtureBase<T>
        where T : class
    {
        private readonly IContainer container;

        public AutofacTestFixture()
        {
            this.container = new ContainerBuilder().Build();
        }

        public TService Resolve<TService>()
        {
            return this.container.Resolve<TService>();
        }

        public void RegisterInstance<TService>(TService service)
            where TService : class
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(service);
            builder.Update(this.container);
        }

        public void RegisterType<TImplementer, TService>()
            where TService : class
            where TImplementer : TService
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TImplementer>().As<TService>();
            builder.Update(this.container);
        }

        protected override void BeforeResolve()
        {
            base.BeforeResolve();

            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(typeof(JSLint).Assembly)
                .Where(IsClass)
                .AsImplementedInterfaces()
                .AsSelf()
                .FindConstructorsWith(x =>
                {
                    return x.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                });

            builder.Update(this.container);
        }

        protected override T Resolve()
        {
            return this.Resolve<T>();
        }

        private static bool IsClass(Type type)
        {
            return type.IsClass &&
                !type.IsAbstract &&
                !type.IsGenericTypeDefinition &&
                !type.Namespace.Contains("Abstraction");
        }
    }
}
