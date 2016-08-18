using System;
using System.Linq;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Resources;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Validators;
using CapsCollection.Desktop.UI.Modules.BulkLoad.ViewModels;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Views;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace CapsCollection.Desktop.Tests.BulkLoadModule
{
    [TestClass]
    public class BulkLoadViewModelTests : TestBase
    {
        private Mock<IBeerLoadView> _beerLoadViewMock = new Mock<IBeerLoadView>();
        private Mock<IBulkLoadView> _bulkLoadViewMock = new Mock<IBulkLoadView>();
        private Mock<IEventAggregator> _eventAggregatorMock = new Mock<IEventAggregator>();
        private Mock<IValidatorFactory> _validatorFactoryMock = new Mock<IValidatorFactory>();
        private Mock<IBeerLoadViewModel> _beerLoadViewModelMock = new Mock<IBeerLoadViewModel>();

        private Mock<BeerSavedEvent> _savedEventMock = new Mock<BeerSavedEvent>();
        private Action<BeerSavedDataEventArgs> _savedCallback;

        private Mock<ImageToLoadEvent> _beerLoadEventMock = new Mock<ImageToLoadEvent>();
        private Action<BeerLoadDataEventArgs> _beerLoadCallback;

        private Mock<BusyEvent> _busyEventMock = new Mock<BusyEvent>();
        private Action<bool> _busyCallback;

        private readonly Mock<BeerErrorEvent> _beerErrorEventMock = new Mock<BeerErrorEvent>();

        private BulkLoadViewModel _bulkLoadViewModel;
        private BeerLoadViewModel _beerLoadViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _validatorFactoryMock.Setup(x => x.GetValidator<BeerLoadViewModel>()).Returns(new BeerLoadViewModelValidator());

            _eventAggregatorMock.Setup(x => x.GetEvent<BeerSavedEvent>()).Returns(_savedEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BusyEvent>()).Returns(_busyEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ImageToLoadEvent>()).Returns(_beerLoadEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerErrorEvent>()).Returns(_beerErrorEventMock.Object);

            _busyEventMock.Setup(x => x.Subscribe(It.IsAny<Action<bool>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<bool>>()))
                                       .Callback<Action<bool>, ThreadOption, bool, Predicate<bool>>((e, t, b, a) => _busyCallback = e);

            _savedEventMock.Setup(x => x.Subscribe(It.IsAny<Action<BeerSavedDataEventArgs>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<BeerSavedDataEventArgs>>()))
                                        .Callback<Action<BeerSavedDataEventArgs>, ThreadOption, bool, Predicate<BeerSavedDataEventArgs>>((e, t, b, a) => _savedCallback = e);

            _beerLoadEventMock.Setup(x => x.Subscribe(It.IsAny<Action<BeerLoadDataEventArgs>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<BeerLoadDataEventArgs>>()))
                                           .Callback<Action<BeerLoadDataEventArgs>, ThreadOption, bool, Predicate<BeerLoadDataEventArgs>>((e, t, b, a) => _beerLoadCallback = e);

            _bulkLoadViewModel = new BulkLoadViewModel(_bulkLoadViewMock.Object, _eventAggregatorMock.Object, _beerLoadViewModelMock.Object);
            _beerLoadViewModel = new BeerLoadViewModel(_beerLoadViewMock.Object, _eventAggregatorMock.Object, _validatorFactoryMock.Object);
        }


        [TestMethod]
        public void BulkLoadViewModel_ConstructorInitialize()
        {
            Assert.IsNotNull(_bulkLoadViewModel.HeaderInfo);
        }
        

        [TestMethod]
        public void BulkLoadViewModel_OnImageRecieved_PrepareViewModel_Null()
        {
            _beerLoadCallback.Invoke(It.IsAny<BeerLoadDataEventArgs>());

            _beerLoadViewModelMock.Verify(x => x.PrepareViewModel(It.IsAny<BeerLoadDataEventArgs>()), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == BulkLoadModuleStrings.PrepareBeerViewForLoadingError)), Times.Once);
            Assert.IsFalse(_bulkLoadViewModel.BeerList.Any());
            Assert.IsTrue(_bulkLoadViewModel.HeaderInfo.Contains("0"));
        }


        [TestMethod]
        public void BulkLoadViewModel_OnImageRecieved_PrepareViewModel_OneCall()
        {
            PrepareThumbnails();
            var beerLoadDataEventArgs = new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails };
            var beerLoadViewModelFirst = _beerLoadViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails });
            _beerLoadViewModelMock.Setup(x => x.PrepareViewModel(beerLoadDataEventArgs)).Returns(beerLoadViewModelFirst);

            _beerLoadCallback.Invoke(beerLoadDataEventArgs);

            _beerLoadViewModelMock.Verify(x => x.PrepareViewModel(It.Is<BeerLoadDataEventArgs>(i => i.ImageList == beerLoadDataEventArgs.ImageList)), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == BulkLoadModuleStrings.PrepareBeerViewForLoadingError)), Times.Never);
            Assert.IsTrue(_bulkLoadViewModel.BeerList.Count == 1);
            Assert.IsTrue(_bulkLoadViewModel.HeaderInfo.Contains("1"));
        }


        [TestMethod]
        public void BulkLoadViewModel_OnImageRecieved_PrepareViewModel_ReturnViewModel_MultipleCallsSameResults()
        {
            PrepareThumbnails();
            var beerLoadDataEventArgs = new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails };
            var beerLoadViewModelFirst = _beerLoadViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails });
            _beerLoadViewModelMock.Setup(x => x.PrepareViewModel(beerLoadDataEventArgs)).Returns(beerLoadViewModelFirst);

            _beerLoadCallback.Invoke(beerLoadDataEventArgs);
            _beerLoadCallback.Invoke(beerLoadDataEventArgs);
            _beerLoadCallback.Invoke(beerLoadDataEventArgs);

            _beerLoadViewModelMock.Verify(x => x.PrepareViewModel(It.Is<BeerLoadDataEventArgs>(i => i.ImageList == beerLoadDataEventArgs.ImageList)), Times.Exactly(3));
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == BulkLoadModuleStrings.PrepareBeerViewForLoadingError)), Times.Never);
            Assert.IsTrue(_bulkLoadViewModel.BeerList.Count == 1);
            Assert.IsTrue(_bulkLoadViewModel.HeaderInfo.Contains("1"));
        }


        [TestMethod]
        public void BulkLoadViewModel_OnImageRecieved_PrepareViewModel_ReturnViewModel_MultipleCallsDiffResults()
        {
            PrepareThumbnails();

            var beerLoadViewModelFirst = _beerLoadViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails });
            _beerLoadViewModelMock.Setup(x => x.PrepareViewModel(It.IsAny<BeerLoadDataEventArgs>())).Returns(beerLoadViewModelFirst);
            _beerLoadCallback.Invoke(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails });

            var beerLoadViewModelSecond = _beerLoadViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsOther });
            _beerLoadViewModelMock.Setup(x => x.PrepareViewModel(It.IsAny<BeerLoadDataEventArgs>())).Returns(beerLoadViewModelSecond);
            _beerLoadCallback.Invoke(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsOther });

            var beerLoadViewModelThird = _beerLoadViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsAnother });
            _beerLoadViewModelMock.Setup(x => x.PrepareViewModel(It.IsAny<BeerLoadDataEventArgs>())).Returns(beerLoadViewModelThird);
            _beerLoadCallback.Invoke(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsAnother });

            _beerLoadViewModelMock.Verify(x => x.PrepareViewModel(It.Is<BeerLoadDataEventArgs>(i => i.ImageList == _imageDataWithThumbnails)), Times.Once);
            _beerLoadViewModelMock.Verify(x => x.PrepareViewModel(It.Is<BeerLoadDataEventArgs>(i => i.ImageList == _imageDataWithThumbnailsOther)), Times.Once);
            _beerLoadViewModelMock.Verify(x => x.PrepareViewModel(It.Is<BeerLoadDataEventArgs>(i => i.ImageList == _imageDataWithThumbnailsAnother)), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == BulkLoadModuleStrings.PrepareBeerViewForLoadingError)), Times.Never);
            Assert.IsTrue(_bulkLoadViewModel.BeerList.Count == 3);
            Assert.IsTrue(_bulkLoadViewModel.HeaderInfo.Contains("3"));
        }


        [TestMethod]
        public void BulkLoadViewModel_OnBusyStatusRecieved()
        {
            _busyCallback.Invoke(true);
            Assert.IsTrue(_bulkLoadViewModel.IsBusy);

            _busyCallback.Invoke(false);
            Assert.IsFalse(_bulkLoadViewModel.IsBusy);
        }


        [TestMethod]
        public void BulkLoadViewModel_OnBeerLoadedRecieved()
        {
            _savedCallback.Invoke(It.IsAny<BeerSavedDataEventArgs>());
        }


        [TestMethod]
        public void BulkLoadViewModel_OnBeerLoadedRecieved_BeerRemoved()
        {
            PrepareThumbnails();

            var beerLoadViewModelFirst = _beerLoadViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails });
            var beerLoadViewModelSecond = _beerLoadViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsOther });
            var beerLoadViewModelThird = _beerLoadViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsAnother });

            _bulkLoadViewModel.BeerList.Add(beerLoadViewModelFirst);
            _bulkLoadViewModel.BeerList.Add(beerLoadViewModelSecond);
            _bulkLoadViewModel.BeerList.Add(beerLoadViewModelThird);

            _savedCallback.Invoke(new BeerSavedDataEventArgs() { BeerTempId = beerLoadViewModelSecond.BeerTempId });

            CollectionAssert.DoesNotContain(_bulkLoadViewModel.BeerList, beerLoadViewModelSecond);
            Assert.IsTrue(_bulkLoadViewModel.BeerList.Count == 2);
        }
    }
}
