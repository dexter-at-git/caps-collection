using System.Collections.Generic;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Home.Models;

namespace CapsCollection.Desktop.UI.Modules.Home.ViewModels
{
    public interface IImageTypeAggregator
    {
        void PutImageType(string imagesPath, ImageType imageType);
        void RemoveImageType(ImageType imageType);
        Dictionary<int, List<ImageData>> CombineImages();
        ImageTypeStatistics GetImageTypeStaticstics();
    }
}