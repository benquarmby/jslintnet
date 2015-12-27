namespace JSLintNet.Json
{
    using System;
    using Newtonsoft.Json.Converters;

    internal class JSLintWarningConverter : CustomCreationConverter<IJSLintWarning>
    {
        public override IJSLintWarning Create(Type objectType)
        {
            return new JSLintWarning();
        }
    }
}
