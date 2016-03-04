﻿namespace JSLintNet
{
    using System;

    internal interface IJavaScriptContext : IDisposable
    {
        void InjectScript(string source);

        object InvokeFunction(string function, params object[] args);
    }
}
