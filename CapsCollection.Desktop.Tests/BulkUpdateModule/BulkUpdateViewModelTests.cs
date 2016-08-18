using System;
using System.Linq;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Resources;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Validators;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.ViewModels;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Views;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace CapsCollection.Desktop.Tests.BulkUpdateModule
{
    [TestClass]
    public class BulkUpdateViewModelTests : TestBase
    {
        private Mock<IBeerUpdateView> _beerUpdateViewMock = new Mock<IBeerUpdateView>();
        private Mock<IBulkUpdateView> _bulkUpdateViewMock = new Mock<IBulkUpdateView>();
        private Mock<IEventAggregator> _eventAggregatorMock = new Mock<IEventAggregator>();
        private Mock<IValidatorFactory> _validatorFactoryMock = new Mock<IValidatorFactory>();
        private Mock<IBeerUpdateViewModel> _beerUpdateViewModelMock = new Mock<IBeerUpdateViewModel>();

        private Mock<BeerSavedEvent> _savedEventMock = new Mock<BeerSavedEvent>();
        private Action<BeerSavedDataEventArgs> _savedCallback;

        private Mock<ImageToUpdateEvent> _beerUpdateEventMock = new Mock<ImageToUpdateEvent>();
        private Action<BeerLoadDataEventArgs> _beerUpdateCallback;

        private Mock<BusyEvent> _busyEventMock = new Mock<BusyEvent>();
        private Action<bool> _busyCallback;

        private readonly Mock<BeerErrorEvent> _beerErrorEventMock = new Mock<BeerErrorEvent>();

        private BulkUpdateViewModel _bulkUpdateViewModel;
        private BeerUpdateViewModel _beerUpdateViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _validatorFactoryMock.Setup(x => x.GetValidator<BeerUpdateViewModel>()).Returns(new BeerUpdateViewModelValidator());

            _eventAggregatorMock.Setup(x => x.GetEvent<BeerSavedEvent>()).Returns(_savedEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BusyEvent>()).Returns(_busyEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ImageToUpdateEvent>()).Returns(_beerUpdateEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerErrorEvent>()).Returns(_beerErrorEventMock.Object);

            _busyEventMock.Setup(x => x.Subscribe(It.IsAny<Action<bool>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<bool>>()))
                                       .Callback<Action<bool>, ThreadOption, bool, Predicate<bool>>((e, t, b, a) => _busyCallback = e);

            _savedEventMock.Setup(x => x.Subscribe(It.IsAny<Action<BeerSavedDataEventArgs>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<BeerSavedDataEventArgs>>()))
                                        .Callback<Action<BeerSavedDataEventArgs>, ThreadOption, bool, Predicate<BeerSavedDataEventArgs>>((e, t, b, a) => _savedCallback = e);

            _beerUpdateEventMock.Setup(x => x.Subscribe(It.IsAny<Action<BeerLoadDataEventArgs>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<BeerLoadDataEventArgs>>()))
                                           .Callback<Action<BeerLoadDataEventArgs>, ThreadOption, bool, Predicate<BeerLoadDataEventArgs>>((e, t, b, a) => _beerUpdateCallback = e);

            _bulkUpdateViewModel = new BulkUpdateViewModel(_bulkUpdateViewMock.Object, _eventAggregatorMock.Object, _beerUpdateViewModelMock.Object);
            _beerUpdateViewModel = new BeerUpdateViewModel(_beerUpdateViewMock.Object, _eventAggregatorMock.Object, _validatorFactoryMock.Object);
        }


        [TestMethod]
        public void BulkUpdateViewModel_ConstructorInitialize()
        {
            Assert.IsNotNull(_bulkUpdateViewModel.HeaderInfo);
        }
        

        [TestMethod]
        public void BulkUpdateViewModel_OnImageRecieved_PrepareViewModel_Null()
        {
            _beerUpdateCallback.Invoke(It.IsAny<BeerLoadDataEventArgs>());

            _beerUpdateViewModelMock.Verify(x => x.PrepareViewModel(It.IsAny<BeerLoadDataEventArgs>()), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == BulkUpdateModuleStrings.PrepareBeerViewForUpdatingError)), Times.Once);
            Assert.IsFalse(_bulkUpdateViewModel.BeerList.Any());
            Assert.IsTrue(_bulkUpdateViewModel.HeaderInfo.Contains("0"));
        }


        [TestMethod]
        public void BulkUpdateViewModel_OnImageRecieved_PrepareViewModel_OneCall()
        {
            PrepareThumbnails();
            var beerLoadDataEventArgs = new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails };
            var beerLoadViewModelFirst = _beerUpdateViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails });
            _beerUpdateViewModelMock.Setup(x => x.PrepareViewModel(beerLoadDataEventArgs)).Returns(beerLoadViewModelFirst);

            _beerUpdateCallback.Invoke(beerLoadDataEventArgs);

            _beerUpdateViewModelMock.Verify(x => x.PrepareViewModel(It.Is<BeerLoadDataEventArgs>(i => i.ImageList == beerLoadDataEventArgs.ImageList)), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == BulkUpdateModuleStrings.PrepareBeerViewForUpdatingError)), Times.Never);
            Assert.IsTrue(_bulkUpdateViewModel.BeerList.Count == 1);
            Assert.IsTrue(_bulkUpdateViewModel.HeaderInfo.Contains("1"));
        }


        [TestMethod]
        public void BulkUpdateViewModel_OnImageRecieved_PrepareViewModel_ReturnViewModel_MultipleCallsSameResults()
        {
            PrepareThumbnails();
            var beerLoadDataEventArgs = new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails };
            var beerLoadViewModelFirst = _beerUpdateViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails });
            _beerUpdateViewModelMock.Setup(x => x.PrepareViewModel(beerLoadDataEventArgs)).Returns(beerLoadViewModelFirst);

            _beerUpdateCallback.Invoke(beerLoadDataEventArgs);
            _beerUpdateCallback.Invoke(beerLoadDataEventArgs);
            _beerUpdateCallback.Invoke(beerLoadDataEventArgs);

            _beerUpdateViewModelMock.Verify(x => x.PrepareViewModel(It.Is<BeerLoadDataEventArgs>(i => i.ImageList == beerLoadDataEventArgs.ImageList)), Times.Exactly(3));
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == BulkUpdateModuleStrings.PrepareBeerViewForUpdatingError)), Times.Never);
            Assert.IsTrue(_bulkUpdateViewModel.BeerList.Count == 1);
            Assert.IsTrue(_bulkUpdateViewModel.HeaderInfo.Contains("1"));
        }


        [TestMethod]
        public void BulkUpdateViewModel_OnImageRecieved_PrepareViewModel_ReturnViewModel_MultipleCallsDiffResults()
        {
            PrepareThumbnails();

            var beerLoadViewModelFirst = _beerUpdateViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails });
            _beerUpdateViewModelMock.Setup(x => x.PrepareViewModel(It.IsAny<BeerLoadDataEventArgs>())).Returns(beerLoadViewModelFirst);
            _beerUpdateCallback.Invoke(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails });

            var beerLoadViewModelSecond = _beerUpdateViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsOther });
            _beerUpdateViewModelMock.Setup(x => x.PrepareViewModel(It.IsAny<BeerLoadDataEventArgs>())).Returns(beerLoadViewModelSecond);
            _beerUpdateCallback.Invoke(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsOther });

            var beerLoadViewModelThird = _beerUpdateViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsAnother });
            _beerUpdateViewModelMock.Setup(x => x.PrepareViewModel(It.IsAny<BeerLoadDataEventArgs>())).Returns(beerLoadViewModelThird);
            _beerUpdateCallback.Invoke(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsAnother });

            _beerUpdateViewModelMock.Verify(x => x.PrepareViewModel(It.Is<BeerLoadDataEventArgs>(i => i.ImageList == _imageDataWithThumbnails)), Times.Once);
            _beerUpdateViewModelMock.Verify(x => x.PrepareViewModel(It.Is<BeerLoadDataEventArgs>(i => i.ImageList == _imageDataWithThumbnailsOther)), Times.Once);
            _beerUpdateViewModelMock.Verify(x => x.PrepareViewModel(It.Is<BeerLoadDataEventArgs>(i => i.ImageList == _imageDataWithThumbnailsAnother)), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == BulkUpdateModuleStrings.PrepareBeerViewForUpdatingError)), Times.Never);
            Assert.IsTrue(_bulkUpdateViewModel.BeerList.Count == 3);
            Assert.IsTrue(_bulkUpdateViewModel.HeaderInfo.Contains("3"));
        }


        [TestMethod]
        public void BulkUpdateViewModel_OnBusyStatusRecieved()
        {
            _busyCallback.Invoke(true);
            Assert.IsTrue(_bulkUpdateViewModel.IsBusy);

            _busyCallback.Invoke(false);
            Assert.IsFalse(_bulkUpdateViewModel.IsBusy);
        }


        [TestMethod]
        public void BulkUpdateViewModel_OnBeerUpdateedRecieved()
        {
            _savedCallback.Invoke(It.IsAny<BeerSavedDataEventArgs>());
        }


        [TestMethod]
        public void BulkUpdateViewModel_OnBeerUpdateedRecieved_BeerRemoved()
        {
            PrepareThumbnails();

            var beerLoadViewModelFirst = _beerUpdateViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnails });
            var beerLoadViewModelSecond = _beerUpdateViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsOther });
            var beerLoadViewModelThird = _beerUpdateViewModel.PrepareViewModel(new BeerLoadDataEventArgs() { ImageList = _imageDataWithThumbnailsAnother });

            _bulkUpdateViewModel.BeerList.Add(beerLoadViewModelFirst);
            _bulkUpdateViewModel.BeerList.Add(beerLoadViewModelSecond);
            _bulkUpdateViewModel.BeerList.Add(beerLoadViewModelThird);

            _savedCallback.Invoke(new BeerSavedDataEventArgs() { BeerTempId = beerLoadViewModelSecond.BeerTempId });

            CollectionAssert.DoesNotContain(_bulkUpdateViewModel.BeerList, beerLoadViewModelSecond);
            Assert.IsTrue(_bulkUpdateViewModel.BeerList.Count == 2);
        }
    }
}
