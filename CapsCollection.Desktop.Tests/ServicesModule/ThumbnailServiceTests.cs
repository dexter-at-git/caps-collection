using System.IO;
using System.Linq;
using CapsCollection.Desktop.UI.Modules.Services;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapsCollection.Desktop.Tests.ServicesModule
{
    [TestClass]
    public class ThumbnailServiceTests : TestBase
    {
        private IThumbnailService _thumbnailService;

        [TestInitialize]
        public void TestInitialize()
        {
            _thumbnailService = new ThumbnailService();
        }


        [TestMethod]
        public void ThumbnailService_EmptyByteInput()
        {
            var thumbnailByte = _thumbnailService.Generate(new byte[] {}, 0, 0);

            Assert.IsTrue(thumbnailByte.Length == 0);
        }


        [TestMethod]
        public void ThumbnailService_InvalidThumbnailSize()
        {
            var thumbnailByte = _thumbnailService.Generate(new byte[255], 0, 0);

            Assert.IsTrue(thumbnailByte.Length == 0);
        }


        [TestMethod]
        public void ThumbnailService_EmptyByteInput_()
        {
            var imageData = _combinedImages.Values.ToList()[0][0];
            var imageBytes = File.ReadAllBytes(imageData.FileInfo.FullName);

            var thumbnailByte = _thumbnailService.Generate(imageBytes, 10, 10);

            Assert.IsFalse(thumbnailByte.Length == 0);
            Assert.IsTrue(thumbnailByte.Length < imageBytes.Length);
        }
    }
}
