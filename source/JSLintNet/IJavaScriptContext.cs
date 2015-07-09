﻿namespace JSLintNet
{
    using System;

    internal interface IJavaScriptContext : IDisposable
    {
        dynamic Script { get; }

        object Run(string source);

        void SetParameter(string name, object value);

        void TerminateExecution();
    }
}
