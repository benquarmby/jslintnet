namespace JSLintNet.QualityTools
{
    using Autofac;

    public interface IDependencyRegistrar
    {
        IDependencyRegistrar RegisterInstance<TService>(TService service)
            where TService : class;

        IDependencyRegistrar RegisterType<TImplementer, TService>()
            where TImplementer : TService
            where TService : class;

        IDependencyRegistrar RegisterModule(Module module);
    }
}
