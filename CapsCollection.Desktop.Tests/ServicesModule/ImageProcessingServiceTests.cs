using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Services;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapsCollection.Desktop.Tests.ServicesModule
{
    [TestClass]
    public class ImageProcessingServiceTests : TestBase
    {
        private ImageProcessingService _imageProcessingService;
        private IThumbnailService _thumbnailService;

        [TestInitialize]
        public void TestInitialize()
        {
            _thumbnailService = new ThumbnailService();
            _imageProcessingService = new ImageProcessingService(_thumbnailService);
        }


        [TestMethod]
        public void ImageProcessingService_Input_Empty()
        {
            var combination = new KeyValuePair<int, List<ImageData>>();

            var thumbnailsList = _imageProcessingService.PrepareThumbnails(combination).Result;

            Assert.IsNotNull(thumbnailsList);
            Assert.IsFalse(thumbnailsList.Any());
        }


        [TestMethod]
        public void ImageProcessingService_Input_EmptyImageList()
        {
            var combination = new KeyValuePair<int, List<ImageData>>(1, new List<ImageData>());

            var thumbnailsList = _imageProcessingService.PrepareThumbnails(combination).Result;

            Assert.IsNotNull(thumbnailsList);
            Assert.IsFalse(thumbnailsList.Any());
        }
        

        [TestMethod]
        public void ImageProcessingService_Input_InvalidImagePath()
        {
            var combination = _combinedImages.ToList()[0];
            foreach (var image in combination.Value)
            {
                image.FileInfo = new FileInfo(_fakePath);
            }

            var thumbnailsList = _imageProcessingService.PrepareThumbnails(combination).Result;

            Assert.IsNotNull(thumbnailsList);
            Assert.IsFalse(thumbnailsList.Any());
        }


        [TestMethod]
        public void ImageProcessingService_Input_ValidImageList()
        {
            var combination = _combinedImages.ToList()[0];

            var thumbnailsList = _imageProcessingService.PrepareThumbnails(combination).Result;

            Assert.IsNotNull(thumbnailsList);
            Assert.AreEqual(combination.Value.Count, thumbnailsList.Count);
            Assert.IsFalse(thumbnailsList.Any(x => x.HiResBytes.Length == 0));
            Assert.IsFalse(thumbnailsList.Any(x => x.PreviewBytes.Length == 0));
            Assert.IsFalse(thumbnailsList.Any(x => x.ThumbnailBytes.Length == 0));
            Assert.IsFalse(thumbnailsList.Any(x => String.IsNullOrEmpty(x.SourceFileFullPath)));
            Assert.IsFalse(thumbnailsList.Any(x => x.ThumbnailBytes.Length >= x.PreviewBytes.Length));
            Assert.IsFalse(thumbnailsList.Any(x => x.ThumbnailBytes.Length >= x.HiResBytes.Length));
            Assert.IsFalse(thumbnailsList.Any(x => x.PreviewBytes.Length >= x.HiResBytes.Length));
        }
    }
}
