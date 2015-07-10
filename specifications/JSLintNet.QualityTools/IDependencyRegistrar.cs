namespace JSLintNet.QualityTools
{
    using System.Reflection;

    public interface IDependencyRegistrar
    {
        IDependencyRegistrar RegisterInstance<TService>(TService service)
            where TService : class;

        IDependencyRegistrar RegisterType<TImplementer, TService>()
            where TImplementer : TService
            where TService : class;

        IDependencyRegistrar RegisterAssemblies(params Assembly[] assemblies);
    }
}
