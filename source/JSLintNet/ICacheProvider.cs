namespace JSLintNet
{
    internal interface ICacheProvider
    {
        void Set(string key, object value, int minutes, params string[] monitorFiles);

        object Get(string key);

        T Get<T>(string key);

        bool Contains(string key);

        void Remove(string key);
    }
}
