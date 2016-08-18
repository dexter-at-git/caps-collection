using System;
using System.Linq;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using CapsCollection.Desktop.UI.Modules.Services.Resources;
using Prism.Events;

namespace CapsCollection.Desktop.UI.Modules.Services
{
    public class EventProcessingService : IEventProcessingService
    {
        private readonly IBeerServiceRepository _beerRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IImageProcessingService _imageProcessingService;

        public EventProcessingService(IBeerServiceRepository beerRepository, IFileRepository fileRepository, IEventAggregator eventAggregator, IImageProcessingService imageProcessingService)
        {
            _beerRepository = beerRepository;
            _fileRepository = fileRepository;
            _eventAggregator = eventAggregator;
            _imageProcessingService = imageProcessingService;

            // Subscribe to events.
            _eventAggregator.GetEvent<ImagesProcessingEvent>().Subscribe(OnImageProcessingDataRecieved);
            _eventAggregator.GetEvent<BeerSavingEvent>().Subscribe(OnSavingDataRecieved);
        }
        
        private async void OnImageProcessingDataRecieved(ImageProcessingDataEventArgs imageProcessingData)
        {
            if (imageProcessingData == null || imageProcessingData.CombinedImages.Count == 0)
            {
                _eventAggregator.GetEvent<BeerErrorEvent>().Publish(new BeerErrorEventArgs() { UserMessage = EventMessages.ImageProcessingDataEmpty, Message = EventMessages.ImageProcessingDataEmpty });
                return;
            }

            var imageList = imageProcessingData.CombinedImages.ToList();
            var checkUpdates = imageProcessingData.CheckUpdates;

            _eventAggregator.GetEvent<BusyEvent>().Publish(true);
            _eventAggregator.GetEvent<BeerLoadingStatusEvent>().Publish(EventMessages.DatabaseFetchingRecords);
            BeerAggregationData beerAggregationData = new BeerAggregationData();
            try
            {
                beerAggregationData = await _beerRepository.GetBeerAggregationData();
            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<BeerErrorEvent>().Publish(new BeerErrorEventArgs() { UserMessage = EventMessages.DatabaseError, Message = ex.Message });
                _eventAggregator.GetEvent<BusyEvent>().Publish(false);
                return;
            }

            // Check that we have all the data.
            if (!beerAggregationData.AllDataCollected())
            {
                _eventAggregator.GetEvent<BeerErrorEvent>().Publish(new BeerErrorEventArgs() { UserMessage = EventMessages.DatabaseAggregationDataMissing, Message = EventMessages.DatabaseAggregationDataMissing });
                _eventAggregator.GetEvent<BusyEvent>().Publish(false);
                return;
            }
            
            // Create loading progress. Maximum progress - number of files to load.
            var loadingProgress = new LoadingProgress();
            loadingProgress.MaximumProgress = imageList.Count;
            loadingProgress.CurrentProgress = 0;

            // Publish loading status to status bar.
            _eventAggregator.GetEvent<BeerLoadingStatusEvent>().Publish(EventMessages.ImageProcessingInProgress);
            
            for (int imageIndex = 0; imageIndex < imageList.Count; imageIndex++)
            {
                var imagesWithThumbnails = await _imageProcessingService.PrepareThumbnails(imageList[imageIndex]);

                // Send event with progress to status bar.
                loadingProgress.CurrentProgress = imageIndex + 1;
                _eventAggregator.GetEvent<BeerLoadingInProgressEvent>().Publish(loadingProgress);

                // Check if processing beer already exist in database.
                var beerMatch = beerAggregationData.ExistingBeers.FirstOrDefault(x => x.BeerId == imageList[imageIndex].Key);

                var beerLoadDataEventArgs = new BeerLoadDataEventArgs()
                {
                    BeerStyles = beerAggregationData.BeerStyles,
                    Countries = beerAggregationData.Countries,
                    CapTypes = beerAggregationData.CapTypes,
                    Breweries = beerAggregationData.Breweries,
                    ImageList = imagesWithThumbnails,
                    Existing = beerMatch
                };

                if (beerMatch != null && checkUpdates)
                {
                    _eventAggregator.GetEvent<ImageToUpdateEvent>().Publish(beerLoadDataEventArgs);
                }
                else
                {
                    _eventAggregator.GetEvent<ImageToLoadEvent>().Publish(beerLoadDataEventArgs);
                }
            }

            _eventAggregator.GetEvent<BusyEvent>().Publish(false);
        }
        
        private async void OnSavingDataRecieved(BeerSavingDataEventArgs beerSavingData)
        {
            if (beerSavingData == null)
            {
                _eventAggregator.GetEvent<BeerErrorEvent>().Publish(new BeerErrorEventArgs() { UserMessage = EventMessages.SavingDataIsEmpty, Message = EventMessages.SavingDataIsEmpty });
                return;
            }

            _eventAggregator.GetEvent<BusyEvent>().Publish(true);
            _eventAggregator.GetEvent<BeerLoadingStatusEvent>().Publish(EventMessages.SavingBeerInProgress);
            
            _eventAggregator.GetEvent<BeerLoadingStatusEvent>().Publish(EventMessages.SavingInDatabase);
            int beerId = 0;
            try
            {
                beerId = await _beerRepository.SaveBeer(beerSavingData.Beer);
            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<BeerErrorEvent>().Publish(new BeerErrorEventArgs() { UserMessage = EventMessages.DatabaseError, Message = ex.Message });
                _eventAggregator.GetEvent<BusyEvent>().Publish(false);
                return;
            }
            
            _eventAggregator.GetEvent<BeerLoadingStatusEvent>().Publish(EventMessages.UploadingBeerImages);

            var uploadPackageBuilder = UploadPackageBuilder.CreateBuilder()
                .AddImagesToPackage(beerSavingData.BottleImage)
                .AddImagesToPackage(beerSavingData.CapImage)
                .AddImagesToPackage(beerSavingData.LabelImage)
                .CreateFileNames(beerId);

            var uloadFiles = uploadPackageBuilder.GetUploadPackage();

            try
            {
                await _beerRepository.UploadImagesToCloud(uloadFiles);
            }
            catch (Exception ex)
            {

                _eventAggregator.GetEvent<BeerErrorEvent>().Publish(new BeerErrorEventArgs() { UserMessage = EventMessages.UploadingBeerImagesError, Message = ex.Message });
                _eventAggregator.GetEvent<BusyEvent>().Publish(false);
                return;
            }

            var renamePackage = uploadPackageBuilder.GetRenamePackage();
            
            _eventAggregator.GetEvent<BeerLoadingStatusEvent>().Publish(EventMessages.RenamingBeerImages);

            try
            {
                await _fileRepository.BatchFileRename(renamePackage);
            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<BeerErrorEvent>().Publish(new BeerErrorEventArgs() { UserMessage = EventMessages.RenamingBeerImagesFailed, Message = ex.Message });
                _eventAggregator.GetEvent<BusyEvent>().Publish(false);
                return;
            }
            
            var beerSavedEventArgs = new BeerSavedDataEventArgs
            {
                BeerTempId = beerSavingData.BeerTempId
            };

            _eventAggregator.GetEvent<BeerSavedEvent>().Publish(beerSavedEventArgs);
            _eventAggregator.GetEvent<BeerLoadingStatusEvent>().Publish(EventMessages.SavingBeerCompleted);
            _eventAggregator.GetEvent<BusyEvent>().Publish(false);
        }
    }
}
