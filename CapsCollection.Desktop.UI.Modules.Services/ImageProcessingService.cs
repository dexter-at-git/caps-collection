using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;

namespace CapsCollection.Desktop.UI.Modules.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly IThumbnailService _thumbnailService;

        public ImageProcessingService(IThumbnailService thumbnailService)
        {
            _thumbnailService = thumbnailService;
        }

        public async Task<List<ImageDataWithThumbnails>> PrepareThumbnails(KeyValuePair<int, List<ImageData>> combinedImages)
        {
            if (combinedImages.Value == null)
            {
                return new List<ImageDataWithThumbnails>();
            }

            var imageList = new List<ImageDataWithThumbnails>();

            await Task.Run(() =>
            {
                foreach (var image in combinedImages.Value)
                {
                    if (!File.Exists(image.FileInfo.FullName))
                    {
                        continue;
                    }

                    byte[] hiResBytes = File.ReadAllBytes(image.FileInfo.FullName);
                    byte[] previewBytes = new byte[] { };
                    byte[] thumbnailBytes = new byte[] { };

                    switch (image.ImageType)
                    {
                        case ImageType.Bottle:
                            previewBytes = _thumbnailService.Generate(hiResBytes, 60, 173);
                            thumbnailBytes = _thumbnailService.Generate(hiResBytes, 45, 130);
                            break;
                        case ImageType.Cap:
                            previewBytes = _thumbnailService.Generate(hiResBytes, 300, 300);
                            thumbnailBytes = _thumbnailService.Generate(hiResBytes, 100, 100);
                            break;
                        case ImageType.Label:
                            previewBytes = _thumbnailService.Generate(hiResBytes, 300, 300);
                            thumbnailBytes = _thumbnailService.Generate(hiResBytes, 100, 100);
                            break;
                    }

                    var imageWithThumbnails = new ImageDataWithThumbnails()
                    {
                        FileIndex = combinedImages.Key,
                        ImageType = image.ImageType,
                        HiResBytes = hiResBytes,
                        PreviewBytes = previewBytes,
                        ThumbnailBytes = thumbnailBytes,
                        SourceFileFullPath = image.FileInfo.FullName
                    };

                    imageList.Add(imageWithThumbnails);
                }
            });

            return imageList;
        }
    }
}