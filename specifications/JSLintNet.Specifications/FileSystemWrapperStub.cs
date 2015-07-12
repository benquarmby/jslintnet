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
            this.TextFiles = new Dictionary<string, string>();
            this.FileResults = new Dictionary<string, List<string>>();
        }

        public Dictionary<string, string> TextFiles { get; set; }

        public Dictionary<string, List<string>> FileResults { get; set; }

        public void AddFile(string directory, string fileName, string contents)
        {
            if (!this.FileResults.ContainsKey(directory))
            {
                this.FileResults[directory] = new List<string>();
            }

            this.FileResults[directory].Add(fileName);
            this.TextFiles.Add(Path.Combine(directory, fileName), contents);
        }

        public string ReadAllText(string path, Encoding encoding)
        {
            return this.TextFiles[path];
        }

        public bool FileExists(string path)
        {
            return this.TextFiles.ContainsKey(path);
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            this.TextFiles[path] = contents;
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
