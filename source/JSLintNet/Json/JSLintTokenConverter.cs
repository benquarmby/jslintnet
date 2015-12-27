namespace JSLintNet.Json
{
    using System;
    using Newtonsoft.Json.Converters;

    internal class JSLintTokenConverter : CustomCreationConverter<IJSLintToken>
    {
        public override IJSLintToken Create(Type objectType)
        {
            return new JSLintToken();
        }
    }
}
