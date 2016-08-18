using System.IO;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;

namespace CapsCollection.Desktop.UI.Modules.Services
{
    public class FileSystem : IFileSystem
    {
        public void MoveFile(string filePath, string newFilePath)
        {
            File.Move(filePath, newFilePath);
        }

        public string PathCombine(string firstPath, string secondPath)
        {
            return Path.Combine(firstPath, secondPath);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}