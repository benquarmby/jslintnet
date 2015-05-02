namespace JSLintNet.Json
{
    using System;
    using JSLintNet.Models;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Creates instances of the specified interface.
    /// </summary>
    internal class JSLintWarningConverter : CustomCreationConverter<IJSLintWarning>
    {
        /// <summary>
        /// Creates the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// A new instance of the specified type.
        /// </returns>
        public override IJSLintWarning Create(Type objectType)
        {
            return new JSLintWarning();
        }
    }
}
