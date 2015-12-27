namespace JSLintNet.Json
{
    using System.Collections.Generic;
    using JSLintNet.Settings;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Provides JSON related services.
    /// </summary>
    internal class JsonProvider : IJsonProvider
    {
        /// <summary>
        /// The serializer settings.
        /// </summary>
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            Error = OnError,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>()
            {
                new StringEnumConverter(),
                new JSLintWarningConverter(),
                new JSLintTokenConverter()
            }
        };

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="value">The value to deserialize.</param>
        /// <returns>
        /// The deserialized object from the JSON string.
        /// </returns>
        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, SerializerSettings);
        }

        /// <summary>
        /// Deserializes the data.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>
        /// A new <see cref="IJSLintData" /> instance.
        /// </returns>
        public IJSLintData DeserializeData(string value)
        {
            return JsonConvert.DeserializeObject<JSLintData>(value, SerializerSettings);
        }

        /// <summary>
        /// Deserializes the settings.
        /// </summary>
        /// <param name="value">The serialized value.</param>
        /// <returns>
        /// A new <see cref="JSLintNetSettings" /> instance.
        /// </returns>
        public JSLintNetSettings DeserializeSettings(string value)
        {
            return JsonConvert.DeserializeObject<JSLintNetSettings>(value, SerializerSettings);
        }

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A serialized JSON string.
        /// </returns>
        public string SerializeObject<T>(T value)
        {
            return JsonConvert.SerializeObject(value, SerializerSettings);
        }

        /// <summary>
        /// Serializes the options.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A serialized JSON string.
        /// </returns>
        public string SerializeOptions(JSLintOptions value)
        {
            return JsonConvert.SerializeObject(value, SerializerSettings);
        }

        /// <summary>
        /// Serializes the settings.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A serialized JSON string.
        /// </returns>
        public string SerializeSettings(JSLintNetSettings value)
        {
            return JsonConvert.SerializeObject(value, SerializerSettings);
        }

        /// <summary>
        /// Called when a JSON error occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
        /// <remarks>Prevents exceptions bubbling up to the consumer.</remarks>
        private static void OnError(object sender, ErrorEventArgs e)
        {
            e.ErrorContext.Handled = true;
        }
    }
}
