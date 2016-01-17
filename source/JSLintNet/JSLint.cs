namespace JSLintNet
{
    using System.Collections.Generic;
    using System.IO;

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
            return Lint(source, null, null);
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
            return Lint(source, options, null);
        }

        /// <summary>
        /// Validates the specified source using JSLint with the provided options and global variables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <param name="globalVariables">The global variables.</param>
        /// <returns>
        /// A <see cref="IJSLintData" /> containing any validation errors.
        /// </returns>
        public static IJSLintData Lint(string source, JSLintOptions options, IList<string> globalVariables)
        {
            using (var context = new JSLintContext())
            {
                return context.Lint(source, options, globalVariables);
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
            return IsJavaScript(fileName) || IsJson(fileName);
        }

        /// <summary>
        /// Determines whether the specified file name has a supported JavaScript extension.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        ///   <c>true</c> if the specified file name has a supported JavaScript extension; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJavaScript(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension == ".js" || extension == "._js";
        }

        /// <summary>
        /// Determines whether the specified file name has a supported JSON extension.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        ///   <c>true</c> if the specified file name has a supported JSON extension; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJson(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension == ".json";
        }
    }
}
