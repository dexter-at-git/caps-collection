using CapsCollection.Silverlight.Infrastructure.ViewModels;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel;
using System.ComponentModel.Composition;
using CapsCollection.Silverlight.Infrastructure.Events;

namespace CapsCollection.Silverlight.UI.Shell.ViewModels
{
    public class BreweryModuleViewModel : ViewModelBase
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion
        

        #region Properties and members

        private bool _isEditBreweriesViewVisible;
        public bool IsEditBreweriesViewVisible
        {
            get { return _isEditBreweriesViewVisible; }
            set
            {
                _isEditBreweriesViewVisible = value;
                RaisePropertyChanged(() => IsEditBreweriesViewVisible);
            }
        }
        
        private bool _isListBreweriesViewVisible;
        public bool IsListBreweriesViewVisible
        {
            get { return _isListBreweriesViewVisible; }
            set
            {
                _isListBreweriesViewVisible = value;
                RaisePropertyChanged(() => IsListBreweriesViewVisible);
            }
        }

        #endregion


        #region Constructors

        public BreweryModuleViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                // Subscribe to events.
                EventAggregator.GetEvent<ShowBreweriesEditRegionEvent>().Subscribe(ShowBreweriesEditRegion);
                EventAggregator.GetEvent<ShowBreweriesListRegionEvent>().Subscribe(ShowBreweriesListRegion);
            }
        }

        #endregion


        #region Commands methods

        public void ShowBreweriesEditRegion(bool obj)
        {
            IsEditBreweriesViewVisible = obj;
        }

        public void ShowBreweriesListRegion(bool obj)
        {
            IsListBreweriesViewVisible = obj;
        }

        #endregion
    }
}
