namespace EnvDTE
{
    using System.Diagnostics.CodeAnalysis;
    using PropertyCollection = EnvDTE.Properties;

    internal static class PropertyCollectionExtensions
    {
        /// <summary>
        /// Gets a named item from the <see cref="Properties"/> collection.
        /// </summary>
        /// <typeparam name="T">The expected value type. If the actual value cannot be unboxed to this type, a runtime exception will occur.</typeparam>
        /// <param name="properties">The properties collection.</param>
        /// <param name="key">The key.</param>
        /// <remarks>This may look like a good candidate for Linq or another functional approach. But manual iteration has proven itself to be the most efficient way to fetch a named item from the very limited <see cref="Properties"/> collection.</remarks>
        /// <returns>The value of the item.</returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Linq is a word.")]
        public static T Get<T>(this PropertyCollection properties, string key)
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

                        break;
                    }
                }
            }

            return default(T);
        }
    }
}
