#pragma warning disable 1591

namespace JSLintNet.Abstractions
{
    using System;
    using System.IO;
    using System.Resources;
    using Microsoft.Build.Framework;

    internal interface ITaskLoggingHelper
    {
        bool HasLoggedErrors { get; }

        string HelpKeywordPrefix { get; set; }

        ResourceManager TaskResources { get; set; }

        string ExtractMessageCode(string message, out string messageWithoutCodePrefix);

        string FormatResourceString(string resourceName, params object[] args);

        string FormatString(string unformatted, params object[] args);

        string GetResourceMessage(string resourceName);

        void LogCommandLine(string commandLine);

        void LogCommandLine(MessageImportance importance, string commandLine);

        void LogError(string message, params object[] messageArgs);

        void LogError(string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);

        void LogErrorFromException(Exception exception);

        void LogErrorFromException(Exception exception, bool showStackTrace);

        void LogErrorFromException(Exception exception, bool showStackTrace, bool showDetail, string file);

        void LogErrorFromResources(string messageResourceName, params object[] messageArgs);

        void LogErrorFromResources(string subcategoryResourceName, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string messageResourceName, params object[] messageArgs);

        void LogErrorWithCodeFromResources(string messageResourceName, params object[] messageArgs);

        void LogErrorWithCodeFromResources(string subcategoryResourceName, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string messageResourceName, params object[] messageArgs);

        void LogExternalProjectFinished(string message, string helpKeyword, string projectFile, bool succeeded);

        void LogExternalProjectStarted(string message, string helpKeyword, string projectFile, string targetNames);

        void LogMessage(string message, params object[] messageArgs);

        void LogMessage(MessageImportance importance, string message, params object[] messageArgs);

        void LogMessageFromResources(string messageResourceName, params object[] messageArgs);

        void LogMessageFromResources(MessageImportance importance, string messageResourceName, params object[] messageArgs);

        bool LogMessageFromText(string lineOfText, MessageImportance messageImportance);

        bool LogMessagesFromFile(string fileName);

        bool LogMessagesFromFile(string fileName, MessageImportance messageImportance);

        bool LogMessagesFromStream(TextReader stream, MessageImportance messageImportance);

        void LogWarning(string message, params object[] messageArgs);

        void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);

        void LogWarningFromException(Exception exception);

        void LogWarningFromException(Exception exception, bool showStackTrace);

        void LogWarningFromResources(string messageResourceName, params object[] messageArgs);

        void LogWarningFromResources(string subcategoryResourceName, string warningCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string messageResourceName, params object[] messageArgs);

        void LogWarningWithCodeFromResources(string messageResourceName, params object[] messageArgs);

        void LogWarningWithCodeFromResources(string subcategoryResourceName, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string messageResourceName, params object[] messageArgs);
    }
}
