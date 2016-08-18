using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using CapsCollection.Common.Settings;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Home.Models;

namespace CapsCollection.Desktop.UI.Modules.Home.ViewModels
{
    public class ImageTypeAggregator : IImageTypeAggregator
    {
        private readonly List<ImageData> _imagesList;

        public ImageTypeAggregator()
        {
            _imagesList = new List<ImageData>();
        }

        public void PutImageType(string imagesPath, ImageType imageType)
        {
            _imagesList.RemoveAll(image => image.ImageType == imageType);

            if (String.IsNullOrEmpty(imagesPath))
            {
                return;
            }

            if (!Directory.Exists(imagesPath))
            {
                return;
            }

            string[] extensions = CapsCollectionSettings.AllowedImageExtensions;

            var filePathes = Directory.EnumerateFiles(imagesPath).Where(file => extensions.Any(file.ToLower().EndsWith)).ToList();

            foreach (var filePath in filePathes)
            {
                FileInfo file = new FileInfo(filePath);

                byte[] checkSum;
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(file.FullName))
                    {
                        checkSum = md5.ComputeHash(stream);
                    }
                }

                var filenameDigits = new string(file.Name.Where(x => Char.IsDigit(x)).ToArray());
                if (filenameDigits.Length == 0)
                {
                    continue;
                }

                _imagesList.Add(new ImageData()
                {
                    FileIndex = Int32.Parse(filenameDigits),
                    FileInfo = file,
                    Md5CheckSum = checkSum,
                    ImageType = imageType
                });
            }
        }

        public void RemoveImageType(ImageType imageType)
        {
            _imagesList.RemoveAll(x => x.ImageType == imageType);
        }

        public ImageTypeStatistics GetImageTypeStaticstics()
        {
            var bottleImagesCount = CountImageType(ImageType.Bottle);
            var capImagesCount = CountImageType(ImageType.Cap);
            var labelImagesCount = CountImageType(ImageType.Label);

            return new ImageTypeStatistics()
            {
                BottleImagesCount = bottleImagesCount,
                CapImagesCount = capImagesCount,
                LabelImagesCount = labelImagesCount
            };
        }

        public Dictionary<int, List<ImageData>> CombineImages()
        {
            return _imagesList.GroupBy(x => x.FileIndex).ToDictionary(k => k.Key, v => v.ToList());
        }

        private int CountImageType(ImageType imageType)
        {
            return _imagesList.Count(x => x.ImageType == imageType);
        }
    }
}