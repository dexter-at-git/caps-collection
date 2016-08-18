using System;
using System.Windows.Input;
using CapsCollection.Desktop.Infrastructure.Resources;
using Prism.Commands;
using Prism.Regions;

namespace CapsCollection.Desktop.UI.Shell.ViewModels
{
    public class UnityShellViewModel
    {
        private readonly IRegionManager _regionManager;

        ICommand _uploadCommand;
        ICommand _settingsCommand;
        ICommand _homeCommand;

        public ICommand UploadCommand
        {
            get { return _uploadCommand ?? (_uploadCommand = new DelegateCommand(NavigateToUploadsExecute)); }
        }

        public ICommand SettingsCommand
        {
            get { return _settingsCommand ?? (_settingsCommand = new DelegateCommand(NavigateToSettingsExecute)); }
        }
        public ICommand HomeCommand
        {
            get { return _homeCommand ?? (_homeCommand = new DelegateCommand(NavigateToHomeView)); }
        }

        public UnityShellViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        private void NavigateToUploadsExecute()
        {
            Uri viewNav = new Uri("BulkLoadView", UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewNav);
        }

        private void NavigateToSettingsExecute()
        {
            Uri viewNav = new Uri("SettingsView", UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewNav);
        }


        private void NavigateToHomeView()
        {
            Uri viewNav = new Uri("TabsView", UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewNav);
        }
    }
}
