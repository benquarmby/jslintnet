#pragma warning disable 1591

namespace JSLintNet.Abstractions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Abstraction for external API.")]
    internal interface IJavaScriptContext : IDisposable
    {
        object GetParameter(string iName);

        object Run(string iSourceCode);

        object Run(string iScript, string iScriptResourceName);

        void SetParameter(string iName, object iObject);

        void TerminateExecution();
    }
}
