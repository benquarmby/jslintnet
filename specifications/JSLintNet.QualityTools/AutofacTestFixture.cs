namespace JSLintNet.QualityTools
{
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

        public IDependencyRegistrar RegisterModule(Module module)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(module);
            builder.Update(this.container);

            return this;
        }

        protected override T Resolve()
        {
            return this.Resolve<T>();
        }
    }
}
