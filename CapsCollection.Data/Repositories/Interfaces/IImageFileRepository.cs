namespace CapsCollection.Data.Repositories.Interfaces
{
    public interface IImageFileRepository
    {
        void UploadImageFile(byte[] imageBytes, string parentContainer, string container, string fileName);
        void DeleteImageFile(string parentContainer, string container, string fileName);
        void UpdateImageFile(string container, string newFileName, string oldFileName);
        void DeleteAllFiles(string imageContainer);
        byte[] DownloadImageFile(string container, string fileName);
    }
}
