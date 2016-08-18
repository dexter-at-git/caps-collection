using System;
using System.IO;
using System.Linq;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Home.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapsCollection.Desktop.Tests.HomeModule
{
    [TestClass]
    public class ImageTypeAggregatorTests : TestBase
    {
        private readonly IImageTypeAggregator _imageTypeAggregator = new ImageTypeAggregator();

        [TestInitialize]
        public void Initialize()
        {
        }


        [TestMethod]
        public void ImageTypeAggregator_PutSameImageTypeMultipleTimes_ShouldNotSum()
        {
            _imageTypeAggregator.PutImageType(_bottlesPath, ImageType.Bottle);
            _imageTypeAggregator.PutImageType(_bottlesPath, ImageType.Bottle);
            _imageTypeAggregator.PutImageType(_bottlesPath, ImageType.Bottle);

            var imageTypeStatistics = _imageTypeAggregator.GetImageTypeStaticstics();

            Assert.AreEqual(_bottlesFilesCount, imageTypeStatistics.BottleImagesCount);
            Assert.AreEqual(0, imageTypeStatistics.CapImagesCount);
            Assert.AreEqual(0, imageTypeStatistics.LabelImagesCount);
        }


        [TestMethod]
        public void ImageTypeAggregator_PutImageTypeWithFakePath_ShouldNotThrowException()
        {
            _imageTypeAggregator.PutImageType(_fakePath, ImageType.Bottle);
        }


        [TestMethod]
        public void ImageTypeAggregator_PutImageTypeWithEmptyPath_ShouldNotThrowException()
        {
            _imageTypeAggregator.PutImageType(null, ImageType.Bottle);
        }


        [TestMethod]
        public void ImageTypeAggregator_PutImageTypeWithFakePath_ImageCount_ShouldBeZero()
        {
            _imageTypeAggregator.PutImageType(_fakePath, ImageType.Bottle);
            _imageTypeAggregator.PutImageType(_capsPath, ImageType.Cap);
            _imageTypeAggregator.PutImageType(_labelsPath, ImageType.Label);

            var imageTypeStatistics = _imageTypeAggregator.GetImageTypeStaticstics();

            Assert.AreEqual(0, imageTypeStatistics.BottleImagesCount);
            Assert.AreEqual(_capsFilesCount, imageTypeStatistics.CapImagesCount);
            Assert.AreEqual(_labelsFilesCount, imageTypeStatistics.LabelImagesCount);
        }


        [TestMethod]
        public void ImageTypeAggregator_PutImageType_ImageCount()
        {
            _imageTypeAggregator.PutImageType(_bottlesPath, ImageType.Bottle);
            _imageTypeAggregator.PutImageType(_capsPath, ImageType.Cap);
            _imageTypeAggregator.PutImageType(_labelsPath, ImageType.Label);

            var imageTypeStatistics = _imageTypeAggregator.GetImageTypeStaticstics();

            Assert.AreEqual(_bottlesFilesCount, imageTypeStatistics.BottleImagesCount);
            Assert.AreEqual(_capsFilesCount, imageTypeStatistics.CapImagesCount);
            Assert.AreEqual(_labelsFilesCount, imageTypeStatistics.LabelImagesCount);
        }


        [TestMethod]
        public void ImageTypeAggregator_GetImageTypeStatistics_OnEmptyCollection_ShouldNotThrowExceptions()
        {
            var imageTypeStatistics = _imageTypeAggregator.GetImageTypeStaticstics();

            Assert.AreEqual(0, imageTypeStatistics.BottleImagesCount);
            Assert.AreEqual(0, imageTypeStatistics.CapImagesCount);
            Assert.AreEqual(0, imageTypeStatistics.LabelImagesCount);
        }


        [TestMethod]
        public void ImageTypeAggregator_RemoveImageType_OnEmptyCollection_ShouldNotThrowExceptions()
        {
            _imageTypeAggregator.RemoveImageType(ImageType.Bottle);

            var imageTypeStatistics = _imageTypeAggregator.GetImageTypeStaticstics();

            Assert.AreEqual(0, imageTypeStatistics.BottleImagesCount);
            Assert.AreEqual(0, imageTypeStatistics.CapImagesCount);
            Assert.AreEqual(0, imageTypeStatistics.LabelImagesCount);
        }


        [TestMethod]
        public void ImageTypeAggregator_RemoveImageType()
        {
            _imageTypeAggregator.PutImageType(_bottlesPath, ImageType.Bottle);
            _imageTypeAggregator.PutImageType(_capsPath, ImageType.Cap);
            _imageTypeAggregator.PutImageType(_labelsPath, ImageType.Label);

            var imageTypeStatistics = _imageTypeAggregator.GetImageTypeStaticstics();

            Assert.AreEqual(_bottlesFilesCount, imageTypeStatistics.BottleImagesCount);
            Assert.AreEqual(_capsFilesCount, imageTypeStatistics.CapImagesCount);
            Assert.AreEqual(_labelsFilesCount, imageTypeStatistics.LabelImagesCount);

            _imageTypeAggregator.RemoveImageType(ImageType.Bottle);
            _imageTypeAggregator.RemoveImageType(ImageType.Label);

            imageTypeStatistics = _imageTypeAggregator.GetImageTypeStaticstics();

            Assert.AreEqual(0, imageTypeStatistics.BottleImagesCount);
            Assert.AreEqual(_capsFilesCount, imageTypeStatistics.CapImagesCount);
            Assert.AreEqual(0, imageTypeStatistics.LabelImagesCount);
        }


        [TestMethod]
        public void ImageTypeAggregator_CombineImages_OnEmptyCollection_ShouldNotThrowExceptions()
        {
            var combinedImages = _imageTypeAggregator.CombineImages();

            Assert.IsFalse(combinedImages.Any());
        }


        [TestMethod]
        public void ImageTypeAggregator_CombineImages_ShouldCount()
        {
            _imageTypeAggregator.PutImageType(_bottlesPath, ImageType.Bottle);
            _imageTypeAggregator.PutImageType(_capsPath, ImageType.Cap);
            _imageTypeAggregator.PutImageType(_labelsPath, ImageType.Label);

            var combinedImages = _imageTypeAggregator.CombineImages();

            var fileList = Directory.GetFiles(_bottlesPath)
                            .Concat(Directory.GetFiles(_capsPath))
                            .Concat(Directory.GetFiles(_bottlesPath)).ToList();

            var uniqueFileIndexes = fileList.Select(x => new string(x.Where(c => Char.IsDigit(c)).ToArray())).Distinct().ToList();

            Assert.AreEqual(uniqueFileIndexes.Count, combinedImages.Count);
        }


        [TestMethod]
        public void ImageTypeAggregator_CombineImages_ShouldHaveValidFilePath()
        {
            _imageTypeAggregator.PutImageType(_bottlesPath, ImageType.Bottle);
            _imageTypeAggregator.PutImageType(_capsPath, ImageType.Cap);
            _imageTypeAggregator.PutImageType(_labelsPath, ImageType.Label);

            var combinedImages = _imageTypeAggregator.CombineImages();

            var filePathList = combinedImages.SelectMany(x => x.Value).Select(x => x.FileInfo).ToList();

            Assert.IsFalse(filePathList.Any(x => !x.Exists));
        }
    }
}
