namespace CapsCollection.Desktop.UI.Modules.Services.Interfaces
{
    public interface IThumbnailService
    {
        byte[] Generate(byte[] imageBytes, int thumbWidth, int thumbHeight);
    }
}