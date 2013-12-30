namespace EnvDTE
{
    internal static class PropertiesExtensions
    {
        public static T Get<T>(this Properties properties, string key)
        {
            if (properties != null)
            {
                foreach (Property property in properties)
                {
                    if (property.Name == key)
                    {
                        var raw = property.Value;

                        if (raw != null)
                        {
                            return (T)raw;
                        }
                    }
                }
            }

            return default(T);
        }
    }
}
