#pragma warning disable 1591

namespace JSLintNet.Abstractions
{
    using System.IO;
    using System.Text;

    internal class FileSystemWrapper : IFileSystemWrapper
    {
        public string ReadAllText(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            File.WriteAllText(path, contents, encoding);
        }

        public string ResolveFile(string path)
        {
            return new FileInfo(path).FullName;
        }

        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }

        public string ResolveDirectory(string path)
        {
            return new DirectoryInfo(path).FullName;
        }
    }
}
