#pragma warning disable 1591

namespace JSLintNet.Abstractions
{
    using System.IO;
    using System.Text;

    internal interface IFileSystemWrapper
    {
        string ReadAllText(string path, Encoding encoding);

        bool FileExists(string path);

        void WriteAllText(string path, string contents, Encoding encoding);

        string ResolveFile(string path);

        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);

        string ResolveDirectory(string path);
    }
}
