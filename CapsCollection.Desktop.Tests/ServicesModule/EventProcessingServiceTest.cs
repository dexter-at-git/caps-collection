using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Services;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using CapsCollection.Desktop.UI.Modules.Services.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace CapsCollection.Desktop.Tests.ServicesModule
{
    [TestClass]
    public class EventProcessingServiceTest : TestBase
    {
        private EventProcessingService _eventProcessingService;

        private readonly Mock<IEventAggregator> _eventAggregatorMock = new Mock<IEventAggregator>();
        private readonly Mock<IBeerServiceRepository> _beerRepositoryMock = new Mock<IBeerServiceRepository>();
        private readonly Mock<IFileRepository> _fileRepositoryMock = new Mock<IFileRepository>();
        private readonly Mock<IImageProcessingService> _imageProcessingService = new Mock<IImageProcessingService>();

        private readonly Mock<ImagesProcessingEvent> _imageProcessingEventMock = new Mock<ImagesProcessingEvent>();
        private Action<ImageProcessingDataEventArgs> _imageProcessingDataCallback;

        private readonly Mock<BeerSavingEvent> _savingEventMock = new Mock<BeerSavingEvent>();
        private Action<BeerSavingDataEventArgs> _savingCallback;

        private readonly Mock<BeerSavedEvent> _savedEventMock = new Mock<BeerSavedEvent>();

        private readonly Mock<BeerLoadingStatusEvent> _beerLoadingStatusEventMock = new Mock<BeerLoadingStatusEvent>();

        private readonly Mock<BeerErrorEvent> _beerErrorEventMock = new Mock<BeerErrorEvent>();

        private readonly Mock<BeerLoadingInProgressEvent> _beerLoadingInPorgressEventMock = new Mock<BeerLoadingInProgressEvent>();

        private readonly Mock<BusyEvent> _busyEventMock = new Mock<BusyEvent>();

        private readonly Mock<ImageToLoadEvent> _imageToLoadEventMock = new Mock<ImageToLoadEvent>();

        private readonly Mock<ImageToUpdateEvent> _imageToUpdateEventMock = new Mock<ImageToUpdateEvent>();


        [TestInitialize]
        public void TestInitialize()
        {
            _eventAggregatorMock.Setup(x => x.GetEvent<ImagesProcessingEvent>()).Returns(_imageProcessingEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerSavingEvent>()).Returns(_savingEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerSavedEvent>()).Returns(_savedEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerErrorEvent>()).Returns(_beerErrorEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerLoadingStatusEvent>()).Returns(_beerLoadingStatusEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BeerLoadingInProgressEvent>()).Returns(_beerLoadingInPorgressEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<BusyEvent>()).Returns(_busyEventMock.Object);

            _eventAggregatorMock.Setup(x => x.GetEvent<ImageToUpdateEvent>()).Returns(_imageToUpdateEventMock.Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ImageToLoadEvent>()).Returns(_imageToLoadEventMock.Object);

            _imageProcessingEventMock.Setup(x => x.Subscribe(It.IsAny<Action<ImageProcessingDataEventArgs>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<ImageProcessingDataEventArgs>>()))
                                     .Callback<Action<ImageProcessingDataEventArgs>, ThreadOption, bool, Predicate<ImageProcessingDataEventArgs>>((e, t, b, a) => _imageProcessingDataCallback = e);

            _savingEventMock.Setup(x => x.Subscribe(It.IsAny<Action<BeerSavingDataEventArgs>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<BeerSavingDataEventArgs>>()))
                            .Callback<Action<BeerSavingDataEventArgs>, ThreadOption, bool, Predicate<BeerSavingDataEventArgs>>((e, t, b, a) => _savingCallback = e);

            _beerRepositoryMock.Setup(x => x.GetBeerStyles()).Returns(_beerStyleList);
            _beerRepositoryMock.Setup(x => x.GetCountriesWithBrewery()).Returns(_countryList);
            _beerRepositoryMock.Setup(x => x.GetBreweries()).Returns(_breweryList);
            _beerRepositoryMock.Setup(x => x.GetCapTypes()).Returns(_capTypeList);
            _beerRepositoryMock.Setup(x => x.GetAllBeers()).Returns(_beerMatchList);

            _eventProcessingService = new EventProcessingService(_beerRepositoryMock.Object, _fileRepositoryMock.Object, _eventAggregatorMock.Object, _imageProcessingService.Object);
        }

        [TestMethod]
        public void EventProcessingService_ConstructorInitialization()
        {
            Assert.IsNotNull(_eventProcessingService);
        }


        [TestMethod]
        public void EventProcessingService_ImagesProcessingEvent_IsReceived()
        {
            _imageProcessingDataCallback.Invoke(new ImageProcessingDataEventArgs());

            _eventAggregatorMock.Verify(x => x.GetEvent<ImagesProcessingEvent>(), Times.Once);
        }


        [TestMethod]
        public void EventProcessingService_ImagesProcessingEvent_NullPayload_NoEventsPublished()
        {
            _imageProcessingDataCallback.Invoke(It.IsAny<ImageProcessingDataEventArgs>());

            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.ImageProcessingDataEmpty)), Times.Once);
            _busyEventMock.Verify(x => x.Publish(true), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.DatabaseFetchingRecords), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseAggregationDataMissing)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.ImageProcessingInProgress), Times.Never);
            _beerLoadingInPorgressEventMock.Verify(x => x.Publish(It.IsAny<LoadingProgress>()), Times.Never);
            _imageToLoadEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Never);
            _imageToUpdateEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Never);
            _busyEventMock.Verify(x => x.Publish(false), Times.Never);
        }


        [TestMethod]
        public void EventProcessingService_ImagesProcessingEvent_WithPayload_DatabaseFailed()
        {
            _beerRepositoryMock.Setup(x => x.GetBeerAggregationData()).Throws(new Exception("Database exception"));

            _imageProcessingDataCallback.Invoke(new ImageProcessingDataEventArgs() { CombinedImages = _combinedImages });

            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.ImageProcessingDataEmpty)), Times.Never);
            _busyEventMock.Verify(x => x.Publish(true), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.DatabaseFetchingRecords), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseAggregationDataMissing)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.ImageProcessingInProgress), Times.Never);
            _beerLoadingInPorgressEventMock.Verify(x => x.Publish(It.IsAny<LoadingProgress>()), Times.Never);
            _imageToLoadEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Never);
            _imageToUpdateEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Never);
            _busyEventMock.Verify(x => x.Publish(false), Times.Once);
        }


        [TestMethod]
        public void EventProcessingService_ImagesProcessingEvent_WithPayload_DatabaseReturnsEmptyCollection()
        {
            _beerRepositoryMock.Setup(x => x.GetBeerAggregationData()).Returns(Task.FromResult(new BeerAggregationData()));

            _imageProcessingDataCallback.Invoke(new ImageProcessingDataEventArgs() { CombinedImages = _combinedImages });

            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.ImageProcessingDataEmpty)), Times.Never);
            _busyEventMock.Verify(x => x.Publish(true), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.DatabaseFetchingRecords), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseAggregationDataMissing)), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.ImageProcessingInProgress), Times.Never);
            _beerLoadingInPorgressEventMock.Verify(x => x.Publish(It.IsAny<LoadingProgress>()), Times.Never);
            _imageToLoadEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Never);
            _imageToUpdateEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Never);
            _busyEventMock.Verify(x => x.Publish(false), Times.Once);
        }


        [TestMethod]
        public void EventProcessingService_ImagesProcessingEvent_WithPayload_NoCheckUpdates()
        {
            _beerRepositoryMock.Setup(x => x.GetBeerAggregationData()).Returns(Task.FromResult(_beerAggregationDataWithMatch));

            _imageProcessingDataCallback.Invoke(new ImageProcessingDataEventArgs() { CombinedImages = _combinedImages, CheckUpdates = false });
            
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.ImageProcessingDataEmpty)), Times.Never);
            _busyEventMock.Verify(x => x.Publish(true), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.DatabaseFetchingRecords), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseAggregationDataMissing)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.ImageProcessingInProgress), Times.Once);
            _beerLoadingInPorgressEventMock.Verify(x => x.Publish(It.IsAny<LoadingProgress>()), Times.Exactly(_combinedImages.Count));
            _imageToLoadEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Exactly(_combinedImages.Count));
            _imageToUpdateEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Never);
            _busyEventMock.Verify(x => x.Publish(false), Times.Once);
        }


        [TestMethod]
        public void EventProcessingService_ImagesProcessingEvent_WithPayload_CheckUpdates_HaveMatches()
        {
            _beerRepositoryMock.Setup(x => x.GetBeerAggregationData()).Returns(Task.FromResult(_beerAggregationDataWithMatch));
            var beerMatches = _beerMatchList.Where(x => _combinedImages.Select(y => y.Key).Contains(x.BeerId)).ToList();

            _imageProcessingDataCallback.Invoke(new ImageProcessingDataEventArgs() { CombinedImages = _combinedImages, CheckUpdates = true });
            
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.ImageProcessingDataEmpty)), Times.Never);
            _busyEventMock.Verify(x => x.Publish(true), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.DatabaseFetchingRecords), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseAggregationDataMissing)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.ImageProcessingInProgress), Times.Once);
            _beerLoadingInPorgressEventMock.Verify(x => x.Publish(It.IsAny<LoadingProgress>()), Times.Exactly(_combinedImages.Count));
            _imageToLoadEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Exactly(_combinedImages.Count - beerMatches.Count));
            _imageToUpdateEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Exactly(beerMatches.Count));
            _busyEventMock.Verify(x => x.Publish(false), Times.Once);
        }


        [TestMethod]
        public void EventProcessingService_ImagesProcessingEvent_WithPayload_CheckUpdates_NoMatches()
        {
            _beerRepositoryMock.Setup(x => x.GetBeerAggregationData()).Returns(Task.FromResult(_beerAggregationDataWithNoMatch));
            var beerMatches = _beerNotMatchList.Where(x => _combinedImages.Select(y => y.Key).Contains(x.BeerId)).ToList();

            _imageProcessingDataCallback.Invoke(new ImageProcessingDataEventArgs() { CombinedImages = _combinedImages, CheckUpdates = true });
            
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.ImageProcessingDataEmpty)), Times.Never);
            _busyEventMock.Verify(x => x.Publish(true), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.DatabaseFetchingRecords), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseAggregationDataMissing)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.ImageProcessingInProgress), Times.Once);
            _beerLoadingInPorgressEventMock.Verify(x => x.Publish(It.IsAny<LoadingProgress>()), Times.Exactly(_combinedImages.Count));
            _imageToLoadEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Exactly(_combinedImages.Count - beerMatches.Count));
            _imageToUpdateEventMock.Verify(x => x.Publish(It.IsAny<BeerLoadDataEventArgs>()), Times.Never);
            _busyEventMock.Verify(x => x.Publish(false), Times.Once);
        }



        [TestMethod]
        public void EventProcessingService_SavingBeerEvent_IsReceived()
        {
            PrepareThumbnails();
            _beerRepositoryMock.Setup(x => x.SaveBeer(It.IsAny<BeerDto>())).Returns(Task.FromResult(5));

            var beerSavingEventArgs = new BeerSavingDataEventArgs()
            {
                BottleImage = _bottleImage,
                CapImage = _capImage,
                LabelImage = _labelImage
            };

            _savingCallback.Invoke(beerSavingEventArgs);

            _eventAggregatorMock.Verify(x => x.GetEvent<BeerSavingEvent>(), Times.Once);
        }


        [TestMethod]
        public void EventProcessingService_SavingBeerEvent_PayloadIsEmpty()
        {
            _savingCallback.Invoke(null);

            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.SavingDataIsEmpty)), Times.Once);
            _busyEventMock.Verify(x => x.Publish(true), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingBeerInProgress), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingInDatabase), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.UploadingBeerImages), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.UploadingBeerImagesError)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.RenamingBeerImages), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.RenamingBeerImagesFailed)), Times.Never);
            _savedEventMock.Verify(x => x.Publish(It.IsAny<BeerSavedDataEventArgs>()), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingBeerCompleted), Times.Never);
            _busyEventMock.Verify(x => x.Publish(false), Times.Never);
        }


        [TestMethod]
        public void EventProcessingService_SavingBeerEvent_DatabaseFailed()
        {
            PrepareThumbnails();

            _beerRepositoryMock.Setup(x => x.SaveBeer(It.IsAny<BeerDto>())).Throws(new Exception("Database failed"));

            _savingCallback.Invoke(new BeerSavingDataEventArgs());
            
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.SavingDataIsEmpty)), Times.Never);
            _busyEventMock.Verify(x => x.Publish(true), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingBeerInProgress), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingInDatabase), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.UploadingBeerImages), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.UploadingBeerImagesError)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.RenamingBeerImages), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.RenamingBeerImagesFailed)), Times.Never);
            _savedEventMock.Verify(x => x.Publish(It.IsAny<BeerSavedDataEventArgs>()), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingBeerCompleted), Times.Never);
            _busyEventMock.Verify(x => x.Publish(false), Times.Once);
        }

        [TestMethod]
        public void EventProcessingService_SavingBeerEvent_UploadBeerFailed()
        {
            PrepareThumbnails();
            
            _beerRepositoryMock.Setup(x => x.UploadImagesToCloud(It.IsAny<List<ImageFileOperationDto>>())).Throws(new Exception("Upload failed"));

            _savingCallback.Invoke(new BeerSavingDataEventArgs());

            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.SavingDataIsEmpty)), Times.Never);
            _busyEventMock.Verify(x => x.Publish(true), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingBeerInProgress), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingInDatabase), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.UploadingBeerImages), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.UploadingBeerImagesError)), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.RenamingBeerImages), Times.Never);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.RenamingBeerImagesFailed)), Times.Never);
            _savedEventMock.Verify(x => x.Publish(It.IsAny<BeerSavedDataEventArgs>()), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingBeerCompleted), Times.Never);
            _busyEventMock.Verify(x => x.Publish(false), Times.Once);
        }

        [TestMethod]
        public void EventProcessingService_SavingBeerEvent_RenameBeerFailed()
        {
            PrepareThumbnails();
            
            _fileRepositoryMock.Setup(x => x.BatchFileRename(It.IsAny<List<ImageFileOperationDto>>())).Throws(new Exception("Rename failed"));

            _savingCallback.Invoke(new BeerSavingDataEventArgs());

            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.SavingDataIsEmpty)), Times.Never);
            _busyEventMock.Verify(x => x.Publish(true), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingBeerInProgress), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingInDatabase), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.UploadingBeerImages), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.UploadingBeerImagesError)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.RenamingBeerImages), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.RenamingBeerImagesFailed)), Times.Once);
            _savedEventMock.Verify(x => x.Publish(It.IsAny<BeerSavedDataEventArgs>()), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingBeerCompleted), Times.Never);
            _busyEventMock.Verify(x => x.Publish(false), Times.Once);
        }
        

        [TestMethod]
        public void EventProcessingService_SavingBeerEvent_SaveBeer()
        {
            PrepareThumbnails();

            _beerRepositoryMock.Setup(x => x.SaveBeer(It.IsAny<BeerDto>())).Returns(Task.FromResult(5));

            var beerSavingEventArgs = new BeerSavingDataEventArgs()
            {
                BeerTempId = Guid.NewGuid(),
                BottleImage = _bottleImage,
                CapImage = _capImage,
                LabelImage = _labelImage
            };

            _savingCallback.Invoke(beerSavingEventArgs);

            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.SavingDataIsEmpty)), Times.Never);
            _busyEventMock.Verify(x => x.Publish(true), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingBeerInProgress), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingInDatabase), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.DatabaseError)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.UploadingBeerImages), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.UploadingBeerImagesError)), Times.Never);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.RenamingBeerImages), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.Is<BeerErrorEventArgs>(e => e.UserMessage == EventMessages.RenamingBeerImagesFailed)), Times.Never);
            _savedEventMock.Verify(x => x.Publish(It.Is<BeerSavedDataEventArgs>(c=>c.BeerTempId == beerSavingEventArgs.BeerTempId)), Times.Once);
            _beerLoadingStatusEventMock.Verify(x => x.Publish(EventMessages.SavingBeerCompleted), Times.Once);
            _busyEventMock.Verify(x => x.Publish(false), Times.Once);
            _beerErrorEventMock.Verify(x => x.Publish(It.IsAny<BeerErrorEventArgs>()), Times.Never);
        }

    }
}
