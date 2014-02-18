namespace JSLintNet
{
    using System;
    using System.Runtime.Caching;

    internal class CacheProvider : ICacheProvider
    {
        private static readonly MemoryCache Cache = new MemoryCache(AssemblyInfo.Product);

        public void Set(string key, object value, int minutes, params string[] monitorFiles)
        {
            var item = new CacheItem(key, value);
            var policy = new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromMinutes(minutes)
            };

            if (monitorFiles != null && monitorFiles.Length > 0)
            {
                var monitor = new HostFileChangeMonitor(monitorFiles);
                policy.ChangeMonitors.Add(monitor);
            }

            Cache.Set(item, policy);
        }

        public object Get(string key)
        {
            return Cache.Get(key);
        }

        public T Get<T>(string key)
        {
            return (T)this.Get(key);
        }

        public bool Contains(string key)
        {
            return Cache.Contains(key);
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }
    }
}