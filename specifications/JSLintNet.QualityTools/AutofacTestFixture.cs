namespace JSLintNet.QualityTools
{
    using System;
    using System.Reflection;
    using Autofac;

    public class AutofacTestFixture<T> : TestFixtureBase<T>, IDependencyRegistrar
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

        public IDependencyRegistrar RegisterInstance<TService>(TService service)
            where TService : class
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(service);
            builder.Update(this.container);

            return this;
        }

        public IDependencyRegistrar RegisterType<TImplementer, TService>()
            where TService : class
            where TImplementer : TService
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TImplementer>().As<TService>();
            builder.Update(this.container);

            return this;
        }

        public IDependencyRegistrar RegisterAssemblies(params Assembly[] assemblies)
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(IsClass)
                .AsImplementedInterfaces()
                .AsSelf()
                .FindConstructorsWith(x =>
                {
                    return x.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                });

            builder.Update(this.container);

            return this;
        }

        protected override void BeforeResolve()
        {
            base.BeforeResolve();

            this.RegisterAssemblies(typeof(JSLint).Assembly);
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
