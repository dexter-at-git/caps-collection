using System.Collections.Generic;
using System.Threading.Tasks;
using CapsCollection.Desktop.Infrastructure.Models;

namespace CapsCollection.Desktop.UI.Modules.Services.Interfaces
{
    public interface IImageProcessingService
    {
        Task<List<ImageDataWithThumbnails>> PrepareThumbnails(KeyValuePair<int, List<ImageData>> combinedImages);
    }
}