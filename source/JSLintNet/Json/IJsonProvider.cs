namespace JSLintNet.Json
{
    using System.Collections.Generic;
    using JSLintNet.Models;
    using JSLintNet.Settings;

    /// <summary>
    /// Provides JSON related services.
    /// </summary>
    internal interface IJsonProvider
    {
        /// <summary>
        /// Deserializes the data.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>
        /// A new <see cref="IJSLintData"/> instance.
        /// </returns>
        IJSLintData DeserializeData(string value);

        /// <summary>
        /// Deserializes the settings.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>
        /// A new <see cref="JSLintNetSettings"/> instance.
        /// </returns>
        JSLintNetSettings DeserializeSettings(string value);

        /// <summary>
        /// Serializes the options.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A serialized JSON string.
        /// </returns>
        string SerializeOptions(JSLintOptions value);

        /// <summary>
        /// Serializes the settings.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A serialized JSON string.
        /// </returns>
        string SerializeSettings(JSLintNetSettings value);

        /// <summary>
        /// Serializes the global variables.
        /// </summary>
        /// <param name="global">The global variables.</param>
        /// <returns>
        /// A serialized JSON string.
        /// </returns>
        string SerializeGlobalVariables(IList<string> globalVariables);
    }
}
