using Prism.Commands;

namespace CapsCollection.Desktop.Infrastructure.Commands
{
    public static class GlobalCommands
    {
        public static CompositeCommand RefreshCommand = new CompositeCommand();

        public static CompositeCommand UploadAllImagesToCloudCommand = new CompositeCommand();
    }
}
