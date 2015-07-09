namespace JSLintNet.Specifications
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using JSLintNet.Abstractions;

    public class FileSystemWrapperStub : IFileSystemWrapper
    {
        public FileSystemWrapperStub()
        {
            this.Files = new Dictionary<string, string>();
            this.FileResults = new Dictionary<string, List<string>>();
        }

        public Dictionary<string, string> Files { get; set; }

        public Dictionary<string, List<string>> FileResults { get; set; }

        public string ReadAllText(string path, Encoding encoding)
        {
            return this.Files[path];
        }

        public bool FileExists(string path)
        {
            return this.Files.ContainsKey(path);
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            this.Files[path] = contents;
        }

        public string ResolveFile(string path)
        {
            return path;
        }

        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            if (this.FileResults.ContainsKey(path))
            {
                return this.FileResults[path].ToArray();
            }

            return new string[0];
        }

        public string ResolveDirectory(string path)
        {
            return path;
        }
    }
}
