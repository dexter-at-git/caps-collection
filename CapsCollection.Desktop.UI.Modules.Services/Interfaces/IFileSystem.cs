namespace CapsCollection.Desktop.UI.Modules.Services.Interfaces
{
    public interface IFileSystem
    {
        void MoveFile(string filePath, string newFilePath);
        string PathCombine(string firstPath, string secondPath);
        bool FileExists(string path);
        string GetDirectoryName(string path);
        bool DirectoryExists(string path);
    }
}