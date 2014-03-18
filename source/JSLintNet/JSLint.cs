namespace JSLintNet
{
    using System.IO;
    using JSLintNet.Models;

    /// <summary>
    /// Provides static JSLint validation.
    /// </summary>
    public static class JSLint
    {
        /// <summary>
        /// Validates the specified source using JSLint.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// A <see cref="IJSLintData" /> containing any validation errors.
        /// </returns>
        public static IJSLintData Lint(string source)
        {
            return Lint(source, null);
        }

        /// <summary>
        /// Validates the specified source using JSLint with the provided options.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// A <see cref="IJSLintData" /> containing any validation errors.
        /// </returns>
        public static IJSLintData Lint(string source, JSLintOptions options)
        {
            using (var context = new JSLintContext())
            {
                return context.Lint(source, options);
            }
        }

        /// <summary>
        /// Determines whether this instance can lint the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        ///   <c>true</c> if this instance can lint the specified file name; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanLint(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension == ".js" || extension == ".json" || extension == "._js";
        }
    }
}
