namespace System
{
    using System.Diagnostics.CodeAnalysis;

    internal static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            return serviceProvider.GetService(typeof(T)) as T;
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The IServiceProvider requires this kind of signature to be useful.")]
        public static TOut GetService<TService, TOut>(this IServiceProvider serviceProvider)
            where TService : class
            where TOut : class
        {
            return serviceProvider.GetService(typeof(TService)) as TOut;
        }

        public static bool TryGetService<T>(this IServiceProvider serviceProvider, out T service)
            where T : class
        {
            service = serviceProvider.GetService<T>();

            return service != null;
        }
    }
}
