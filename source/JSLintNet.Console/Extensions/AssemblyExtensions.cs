namespace System.Reflection
{
    /// <summary>
    /// Extension methods for <see cref="Assembly"/> instances.
    /// </summary>
    internal static class AssemblyExtensions
    {
        /// <summary>
        /// Gets an attribute value from the assembly.
        /// </summary>
        /// <typeparam name="T">The attribute type to get.</typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <param name="memberExpression">The member expression.</param>
        /// <returns>
        /// The attribute value.
        /// </returns>
        public static string GetAttributeValue<T>(this Assembly assembly, Func<T, string> memberExpression)
            where T : Attribute
        {
            var attributes = assembly.GetCustomAttributes(typeof(T), false);

            if (attributes.Length > 0)
            {
                return memberExpression.Invoke((T)attributes[0]);
            }

            return string.Empty;
        }
    }
}
