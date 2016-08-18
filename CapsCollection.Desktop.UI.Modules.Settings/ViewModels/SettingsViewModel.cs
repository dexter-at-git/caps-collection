using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Settings.Views;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace CapsCollection.Desktop.UI.Modules.Settings.ViewModels
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        #region Repositories

        private readonly IEventAggregator _eventAggregator;
        private readonly IBeerServiceRepository _repository;
        private readonly IValidator<ImageLookupFolders> _validator;

        #endregion


        #region Commands

        private ICommand _uploadToCloudCommand;
        private ICommand _uploadCancelCommand;
        private ICommand _downloadCommand;
        private ICommand _downloadCancelCommand;
        private ICommand _deleteCommand;

        public ICommand UploadCommand
        {
            get { return _uploadToCloudCommand ?? (_uploadToCloudCommand = new DelegateCommand(UploadToCloud)); }
        }
        public ICommand UploadCancelCommand
        {
            get { return _uploadCancelCommand ?? (_uploadCancelCommand = new DelegateCommand(UploadCancel)); }
        }
        public ICommand DownloadCommand
        {
            get { return _downloadCommand ?? (_downloadCommand = new DelegateCommand(DownloadFromCloud)); }
        }
        public ICommand DownloadCancelCommand
        {
            get { return _downloadCancelCommand ?? (_downloadCancelCommand = new DelegateCommand(DownloadCancel)); }
        }
        public ICommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new DelegateCommand(DeleteFromCloud)); }
        }

        #endregion


        #region Properties

        public InteractionRequest<Confirmation> ShowConfirmation { get; set; }
        private int BeerProgress { get; set; }
        private int FlagsProgress { get; set; }
        private ImageLookupFolders ImageLookupFolders { get; set; }

        private string _uploadMessage;
        public string UploadMessage
        {
            get { return _uploadMessage; }
            set
            {
                _uploadMessage = value;
                OnPropertyChanged(() => UploadMessage);
            }
        }

        private string _downloadMessage;
        public string DownloadMessage
        {
            get { return _downloadMessage; }
            set
            {
                _downloadMessage = value;
                OnPropertyChanged(() => DownloadMessage);
            }
        }

        private string _deleteMessage;
        public string DeleteMessage
        {
            get { return _deleteMessage; }
            set
            {
                _deleteMessage = value;
                OnPropertyChanged(() => DeleteMessage);
            }
        }

        private int _currentProgress;
        public int CurrentProgress
        {
            get { return _currentProgress; }
            private set
            {
                if (_currentProgress != value)
                {
                    _currentProgress = value;
                    OnPropertyChanged(() => CurrentProgress);
                }
            }
        }

        private int _maximumProgress;
        public int MaximumProgress
        {
            get { return _maximumProgress; }
            private set
            {
                if (_maximumProgress != value)
                {
                    _maximumProgress = value;
                    OnPropertyChanged(() => MaximumProgress);
                }
            }
        }

        private bool _isUploading;
        public bool IsUploading
        {
            get { return _isUploading; }
            private set
            {
                if (_isUploading != value)
                {
                    _isUploading = value;
                    OnPropertyChanged(() => IsUploading);
                }
            }
        }

        private bool _isDeleting;
        public bool IsDeleting
        {
            get { return _isDeleting; }
            private set
            {
                if (_isDeleting != value)
                {
                    _isDeleting = value;
                    OnPropertyChanged(() => IsDeleting);
                    // IsBusy = _isUploading || _isDeleting || _isDownloading;
                }
            }
        }

        private bool _isDownloading;
        public bool IsDownloading
        {
            get { return _isDownloading; }
            private set
            {
                if (_isDownloading != value)
                {
                    _isDownloading = value;
                    OnPropertyChanged(() => IsDownloading);
                    //   IsBusy = _isUploading || _isDeleting || _isDownloading;
                }
            }
        }

        private bool _stopUpload = false;
        public bool StopUpload
        {
            get { return _stopUpload; }
            private set
            {
                if (_stopUpload != value)
                {
                    _stopUpload = value;
                    OnPropertyChanged(() => StopUpload);
                }
            }
        }

        private bool _stopDownload = false;
        public bool StopDownload
        {
            get { return _stopDownload; }
            private set
            {
                if (_stopDownload != value)
                {
                    _stopDownload = value;
                    OnPropertyChanged(() => StopDownload);
                }
            }
        }

        private string _bottlesLookupFolder;

        public string BottlesLookupFolder
        {
            get { return _bottlesLookupFolder; }
            set
            {
                if (_bottlesLookupFolder != value)
                {
                    _bottlesLookupFolder = value;
                    OnPropertyChanged(() => BottlesLookupFolder);
                }

                ImageLookupFolders.BottlesLookupFolder = _bottlesLookupFolder;
                ClearErrorFromProperty("BottlesLookupFolder");
                var validationResult = _validator.Validate(ImageLookupFolders, "BottlesLookupFolder");
                if (!validationResult.IsValid)
                {
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));
                }
            }
        }

        #endregion


        #region Constructor

        public SettingsViewModel(ISettingsView view, IEventAggregator eventAggregator, IBeerServiceRepository repository, IValidatorFactory validatorFactory)
            : base(view)
        {
            _eventAggregator = eventAggregator;
            _repository = repository;
            _validator = validatorFactory.GetValidator<ImageLookupFolders>();

            ShowConfirmation = new InteractionRequest<Confirmation>();

            ImageLookupFolders = new ImageLookupFolders();
        }

        #endregion


        #region Button commands

        private async void DeleteFromCloud()
        {
            IsDeleting = true;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    DeleteMessage = string.Format("Deleting all from cloud ...");

                    _repository.EmptyImageContainers();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            IsDeleting = false;

            stopwatch.Stop();
            DeleteMessage = string.Format("Delete completed in {0}.", stopwatch.Elapsed);
        }

        private void UploadCancel()
        {
            UploadMessage = string.Format("Upload stoping. Waiting for uploading last beer ...");
            StopUpload = true;
        }

        private void DownloadCancel()
        {
            DownloadMessage = string.Format("download stoping. Waiting for downloading last beer ...");
            StopDownload = true;
        }

        private void UploadToCloud()
        {
            // Get all beers.
            var allBeers = _repository.GetAllBeers().OrderBy(x => x.BeerId).ToList();

            // Get all flags.
            string flagsPath = @"D:\Dropbox\Caps_Collection_Images\Flags";
            DirectoryInfo flagsDir = new DirectoryInfo(flagsPath);
            var flagFiles = flagsDir.GetFiles();


            MaximumProgress = allBeers.Count + flagFiles.Count();

            UploadBeersToCloud(allBeers);
            UploadFlagsToCloud(flagFiles);
        }

        private void DownloadFromCloud()
        {
            /*
            var beers = _repository.GetAllBeers().OrderBy(x => x.BeerId);
            var beerList = new List<BeerWithImages>();

            foreach (var beerDto in beers)
            {
                var beerImage = new BeerWithImages(beerDto.BeerId);
                beerImage.BeerId = beerDto.BeerId;
                beerImage.BeerName = beerDto.BeerName;
                beerList.Add(beerImage);
            }
            
            DownloadBeersFromCloud(beerList);
            */
        }


        #endregion


        #region Database events

        /*
        private async void DownloadBeersFromCloud(IList<BeerWithImages> beerList)
        {

            var bottlePath = @"D:\Dropbox\Caps_Collection_Images_Downloaded\1.Bottles\";
            var capPath = @"D:\Dropbox\Caps_Collection_Images_Downloaded\2.Caps\";
            var labelPath = @"D:\Dropbox\Caps_Collection_Images_Downloaded\3.Labels\";

            // Delete all files.
            var bottlesDir = new DirectoryInfo(bottlePath);
            foreach (FileInfo file in bottlesDir.GetFiles())
                file.Delete();
            var capsDir = new DirectoryInfo(capPath);
            foreach (FileInfo file in capsDir.GetFiles())
                file.Delete();
            var labelDir = new DirectoryInfo(labelPath);
            foreach (FileInfo file in labelDir.GetFiles())
                file.Delete();


            IsDownloading = true;

            StopDownload = false;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            // For each beer number create async task and download images.
            var beerCount = beerList.Count;
            Task[] tasks = new Task[beerCount];
            var i = 0;
            foreach (var beer in beerList)
            {
                i++;
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                if (StopDownload)
                {
                    source.Cancel();
                    break;
                }

                try
                {
                    var task = Task.Factory
                        .StartNew(() =>
                        {
                            DownloadMessage = string.Format("Downloading \"{0}\" beer image from cloud ({1} of {2}) ...      {3}", beer.BeerName, i, beerCount, stopwatch.Elapsed);

                            DownloadImage(beer, bottlePath, capPath, labelPath);
                        }, TaskCreationOptions.PreferFairness)
                        .ContinueWith((result) =>
                        {

                        }, token, TaskContinuationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());

                    await task;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (StopDownload)
            {
                IsDownloading = false;
            }

            stopwatch.Stop();
            DownloadMessage = string.Format("Downloaded {0} of {1} beer images from cloud in {2}.", i, beerList.Count, stopwatch.Elapsed);
        }
        */
        
        private async void UploadBeersToCloud(IList<BeerDto> beerList)
        {
            IsUploading = true;

            StopUpload = false;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            // For each beer number create async task and generate thumbnails and previews.
            Task[] tasks = new Task[beerList.Count];
            var i = 0;
            foreach (var beer in beerList)
            {
                BeerProgress = i;
                CurrentProgress = BeerProgress + FlagsProgress;
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                if (StopUpload)
                {
                    source.Cancel();
                    break;
                }

                try
                {
                    tasks[i] = Task.Factory
                        .StartNew(() =>
                        {
                            UploadMessage = string.Format("Uploading \"{0}\" beer images to cloud ...      {1}", beer.BeerName, stopwatch.Elapsed);

                            //   LoadBeersToCloud(beer);
                        }, TaskCreationOptions.PreferFairness)
                        .ContinueWith((result) =>
                        {

                        }, token, TaskContinuationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());

                    await tasks[i];
                    i++;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (CurrentProgress == MaximumProgress || StopUpload)
            {
                IsUploading = false;
            }

            stopwatch.Stop();
            UploadMessage = string.Format("Uploaded {0} of {1} beer images to cloud in {2}.", i, beerList.Count, stopwatch.Elapsed);
        }
        
        private async void UploadFlagsToCloud(IList<FileInfo> flagsList)
        {
            IsUploading = true;

            StopUpload = false;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            // For each beer number create async task and generate thumbnails and previews.
            Task[] tasks = new Task[flagsList.Count()];
            var i = 0;
            foreach (var flagFile in flagsList)
            {
                FlagsProgress = i;
                CurrentProgress = BeerProgress + FlagsProgress;
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                if (StopUpload)
                {
                    source.Cancel();
                    break;
                }

                try
                {
                    tasks[i] = Task.Factory
                        .StartNew(() =>
                        {
                            UploadMessage = string.Format("Uploading \"{0}\" flag images to cloud ...      {1}", flagFile.Name, stopwatch.Elapsed);

                            var flagListToLoad = new List<ImageFileOperationDto>();

                            var flagToLoad = new ImageFileOperationDto
                            {
                                Container = "flags",
                                ImageBytes = File.ReadAllBytes(flagFile.FullName),
                                FileName = flagFile.Name,
                                FileOperation = FileOperation.Save
                            };

                            flagListToLoad.Add(flagToLoad);

                            //    _repository.SaveImagesToFileStorage(flagListToLoad);

                        }, TaskCreationOptions.PreferFairness)
                        .ContinueWith((result) =>
                        {

                        }, token, TaskContinuationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());

                    await tasks[i];
                    i++;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (CurrentProgress == MaximumProgress || StopUpload)
            {
                IsUploading = false;
            }

            stopwatch.Stop();
            UploadMessage = string.Format("Uploaded {0} of {1} flag images to cloud in {2}.", i, flagsList.Count(), stopwatch.Elapsed);
        }

        /*
        private void LoadBeersToCloud(BeerDto beer)
        {
            string bottlesPath = @"D:\Dropbox\Caps_Collection_Images\1.Bottles";
            string capsPath = @"D:\Dropbox\Caps_Collection_Images\2.Caps";
            string labelsPath = @"D:\Dropbox\Caps_Collection_Images\3.Labels";


            string fileNumber = beer.BeerId.ToString().PadLeft(5, '0');
            string searchPattern = String.Format("*{0}.*", fileNumber);

            DirectoryInfo bottlesDir = new DirectoryInfo(bottlesPath);
            DirectoryInfo capsDir = new DirectoryInfo(capsPath);
            DirectoryInfo labelsDir = new DirectoryInfo(labelsPath);


            var bottleMatch = bottlesDir.GetFiles(searchPattern);
            var capMatch = capsDir.GetFiles(searchPattern);
            var labelMatch = labelsDir.GetFiles(searchPattern);



            BeerWithImages beerWithImages = new BeerWithImages();

            if (bottleMatch.Length != 0 && bottleMatch[0].Exists)
                beerWithImages.BottleImage.CreateThumbnails(BeerImageType.Bottle, beer.BeerId, bottleMatch[0]);


            if (capMatch.Length != 0 && capMatch[0].Exists)
                beerWithImages.CapImage.CreateThumbnails(BeerImageType.Cap, beer.BeerId, capMatch[0]);


            if (labelMatch.Length != 0 && labelMatch[0].Exists)
                beerWithImages.LabelImage.CreateThumbnails(BeerImageType.Label, beer.BeerId, labelMatch[0]);


            _repository.SaveBeerToFileStorage(beerWithImages);
        }
        */

        /*
        private void DownloadImage(BeerWithImages beerWithImages, string bottlePath, string capPath, string labelPath)
        {
            
            // Bottle bytes.
            var bottleHiResBytes = _repository.DownloadImage(beerWithImages.BottleImage.ImageContainerPath,
                beerWithImages.BottleImage.FileName);
            var bottlePreviewBytes = _repository.DownloadImage(beerWithImages.BottleImage.ImageContainerPath,
                beerWithImages.BottleImage.PreviewFileName);
            var bottleTumbnailBytes = _repository.DownloadImage(beerWithImages.BottleImage.ImageContainerPath,
                beerWithImages.BottleImage.ThumbnailFileName);

            // Bottle hi-res.
            if (bottleHiResBytes.Length > 0)
                using (FileStream fs = new FileInfo(bottlePath + beerWithImages.BottleImage.FileName).Create())
                    fs.Write(bottleHiResBytes, 0, bottleHiResBytes.Length);
            // Bottle preview.
            if (bottlePreviewBytes.Length > 0)
                using (FileStream fs = new FileInfo(bottlePath + beerWithImages.BottleImage.PreviewFileName).Create())
                    fs.Write(bottlePreviewBytes, 0, bottlePreviewBytes.Length);
            // Bottle thumbnail.
            if (bottleTumbnailBytes.Length > 0)
                using (FileStream fs = new FileInfo(bottlePath + beerWithImages.BottleImage.ThumbnailFileName).Create())
                    fs.Write(bottleTumbnailBytes, 0, bottleTumbnailBytes.Length);


            // Cap bytes.
            var capHiResBytes = _repository.DownloadImage(beerWithImages.CapImage.ImageContainerPath,
                beerWithImages.CapImage.FileName);
            var capPreviewBytes = _repository.DownloadImage(beerWithImages.CapImage.ImageContainerPath,
                beerWithImages.CapImage.PreviewFileName);
            var capTumbnailBytes = _repository.DownloadImage(beerWithImages.CapImage.ImageContainerPath,
                beerWithImages.CapImage.ThumbnailFileName);

            // Cap hi-res.
            if (capHiResBytes.Length > 0)
                using (FileStream fs = new FileInfo(capPath + beerWithImages.CapImage.FileName).Create())
                    fs.Write(capHiResBytes, 0, capHiResBytes.Length);
            // Cap preview.
            if (capPreviewBytes.Length > 0)
                using (FileStream fs = new FileInfo(capPath + beerWithImages.CapImage.PreviewFileName).Create())
                    fs.Write(capPreviewBytes, 0, capPreviewBytes.Length);
            // Cap thumbnail.
            if (capTumbnailBytes.Length > 0)
                using (FileStream fs = new FileInfo(capPath + beerWithImages.CapImage.ThumbnailFileName).Create())
                    fs.Write(capTumbnailBytes, 0, capTumbnailBytes.Length);


            // Label bytes.
            var labelHiResBytes = _repository.DownloadImage(beerWithImages.LabelImage.ImageContainerPath,
                beerWithImages.LabelImage.FileName);
            var labelPreviewBytes = _repository.DownloadImage(beerWithImages.LabelImage.ImageContainerPath,
                beerWithImages.LabelImage.PreviewFileName);
            var labelTumbnailBytes = _repository.DownloadImage(beerWithImages.LabelImage.ImageContainerPath,
                beerWithImages.LabelImage.ThumbnailFileName);

            // Label hi-res.
            if (labelHiResBytes.Length > 0)
                using (FileStream fs = new FileInfo(labelPath + beerWithImages.LabelImage.FileName).Create())
                    fs.Write(labelHiResBytes, 0, labelHiResBytes.Length);
            // Label preview.
            if (labelPreviewBytes.Length > 0)
                using (FileStream fs = new FileInfo(labelPath + beerWithImages.LabelImage.PreviewFileName).Create())
                    fs.Write(labelPreviewBytes, 0, labelPreviewBytes.Length);
            // Label thumbnail.
            if (labelTumbnailBytes.Length > 0)
                using (FileStream fs = new FileInfo(labelPath + beerWithImages.LabelImage.ThumbnailFileName).Create())
                    fs.Write(labelTumbnailBytes, 0, labelTumbnailBytes.Length);
            
        }
*/
        /*
         private void LoadCountries()
         {
             const string flagContainerName = "flags";
             const string path = @"D:\Dropbox\Caps_Collection_Images\Flags";

             // Delete all flag files from storage
             _repository.EmptyImageContainers(flagContainerName);

             // Get countries from database
             var countries = _repository.GetAllCountries();
             var countriesWithAlpha = countries.Where(x => x.Alpha3.Trim() != string.Empty);

             foreach (var country in countriesWithAlpha)
             {
                 var flagsToLoad = new List<ImageFileOperationDto>();

                 var flagFileName = string.Format(@"{0}_full.png", country.Alpha3);
                 var flagRoundFileName = string.Format(@"{0}_round.png", country.Alpha3);
                 var flagSquareFileName = string.Format(@"{0}_square.png", country.Alpha3);

                 var flagPath = string.Format(@"{0}\{1}", path, flagFileName);
                 var flagRoundPath = string.Format(@"{0}\{1}", path, flagRoundFileName);
                 var flagSquarePath = string.Format(@"{0}\{1}", path, flagSquareFileName);

                 if (country.Flag.Length > 0)
                 {
                     File.WriteAllBytes(flagPath, country.Flag);
                     flagsToLoad.Add(new ImageFileOperationDto()
                                     {
                                         container = flagContainerName,
                                         fileName = flagFileName,
                                         fileOperation = FileOperation.Save,
                                         imageBytes = country.Flag
                                     });
                 }

                 if (country.FlagRound.Length > 0)
                 {
                     File.WriteAllBytes(flagRoundPath, country.FlagRound);
                     flagsToLoad.Add(new ImageFileOperationDto()
                     {
                         container = flagContainerName,
                         fileName = flagRoundFileName,
                         fileOperation = FileOperation.Save,
                         imageBytes = country.FlagRound
                     });
                 }

                 if (country.FlagSquare.Length > 0)
                 {
                     File.WriteAllBytes(flagSquarePath, country.FlagSquare);
                     flagsToLoad.Add(new ImageFileOperationDto()
                     {
                         container = flagContainerName,
                         fileName = flagSquareFileName,
                         fileOperation = FileOperation.Save,
                         imageBytes = country.FlagSquare
                     });
                 }

                 _repository.SaveImagesToFileStorage(flagsToLoad);
             }
         }
         */

        #endregion
    }
}
