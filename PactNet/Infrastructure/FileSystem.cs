using System.IO;

namespace PactNet.Infrastructure
{
    internal interface IDirectory
    {
        void CreateDirectory(string path);
    }

    internal interface IFile
    {
        Stream Open(string path, FileMode mode, FileAccess access, FileShare share);

        string ReadAllText(string path);

        void WriteAllText(string path, string contents);
    }

    internal interface IFileSystem
    {
        IDirectory Directory { get;  }

        IFile File { get;  }
    }

    internal class FileSystem : IFileSystem
    {
        private class DirectoryWrapper : IDirectory
        {
            public void CreateDirectory(string path)
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }

        public class FileWrapper : IFile
        {
            public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
            {
                return System.IO.File.Open(path, mode, access, share);
            }

            public string ReadAllText(string path)
            {
                return System.IO.File.ReadAllText(path);
            }

            public void WriteAllText(string path, string contents)
            {
                System.IO.File.WriteAllText(path, contents);
            }
        }

        public IDirectory Directory { get; } = new DirectoryWrapper();

        public IFile File { get; } = new FileWrapper();
    }
}