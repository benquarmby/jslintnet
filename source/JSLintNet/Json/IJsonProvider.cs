namespace JSLintNet.Json
{
    using JSLintNet.Settings;

    /// <summary>
    /// Provides JSON related services.
    /// </summary>
    internal interface IJsonProvider
    {
        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="value">The value to deserialize.</param>
        /// <returns>
        /// The deserialized object from the JSON string.
        /// </returns>
        T DeserializeObject<T>(string value);

        /// <summary>
        /// Deserializes the data.
        /// </summary>
        /// <param name="value">The value to deserialize.</param>
        /// <returns>
        /// The deserialized <see cref="IJSLintData"/> from the JSON string.
        /// </returns>
        IJSLintData DeserializeData(string value);

        /// <summary>
        /// Deserializes the settings.
        /// </summary>
        /// <param name="value">The value to deserialize.</param>
        /// <returns>
        /// The deserialized <see cref="JSLintNetSettings"/> from the JSON string.
        /// </returns>
        JSLintNetSettings DeserializeSettings(string value);

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A serialized JSON string.
        /// </returns>
        string SerializeObject<T>(T value);

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
    }
}
