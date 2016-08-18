using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.StatusBar.Views;
using System;
using System.Diagnostics;
using CapsCollection.Desktop.UI.Modules.StatusBar.Resources;
using Prism.Events;

namespace CapsCollection.Desktop.UI.Modules.StatusBar.ViewModels
{
    public class StatusBarViewModel : ViewModelBase, IStatusBarViewModel
    {
        private IEventAggregator _eventAggregator;

        private Stopwatch _stopWatch;

        public StatusBarModuleStrings StatusBarModuleStrings
        {
            get { return new StatusBarModuleStrings(); }
        }

        private string _message = StatusBarModuleStrings.Ready;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(() => Message);
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

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(() => IsLoading);
                }
            }
        }

        public StatusBarViewModel(IStatusBarView view, IEventAggregator eventAggregator) : base(view)
        {
            _eventAggregator = eventAggregator;

            _stopWatch = new Stopwatch();

            _eventAggregator.GetEvent<BeerLoadingStatusEvent>().Subscribe(OnStatusUpdateRecieved);
            _eventAggregator.GetEvent<BeerErrorEvent>().Subscribe(OnErrorRecieved);
            _eventAggregator.GetEvent<BeerLoadingInProgressEvent>().Subscribe(ProgressChanged);
        }

        private void ProgressChanged(LoadingProgress loadingProgress)
        {
            CurrentProgress = loadingProgress.CurrentProgress;
            MaximumProgress = loadingProgress.MaximumProgress;
            
            if (CurrentProgress == 1)
            {
                _stopWatch.Start();
            }
            
            if (CurrentProgress != MaximumProgress)
            {
                IsLoading = true;
            }
            else
            {
                IsLoading = false;
                CurrentProgress = 0;
                
                _stopWatch.Stop();
                var executionTime = Math.Round(_stopWatch.Elapsed.TotalSeconds, 2);

                _stopWatch.Reset();
                Message = String.Format("{0}: {1} sec.", StatusBarModuleStrings.LoadingTime, executionTime);
            }
        }
        
        private void OnStatusUpdateRecieved(string obj)
        {
            Message = obj;
        }

        private void OnErrorRecieved(BeerErrorEventArgs obj)
        {
            Message = String.Format("{0}: {1}", obj.UserMessage, obj.Message);
        }
    }
}
