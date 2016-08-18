using CapsCollection.Silverlight.Infrastructure.ViewModels;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel;
using System.ComponentModel.Composition;
using CapsCollection.Silverlight.Infrastructure.Events;

namespace CapsCollection.Silverlight.UI.Shell.ViewModels
{

    public class CollectionModuleViewModel : ViewModelBase
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }

        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion
        

        #region Properties and Members

        private bool _isEditBeerViewVisible;
        public bool IsEditBeerViewVisible
        {
            get { return _isEditBeerViewVisible; }
            set
            {
                _isEditBeerViewVisible = value;
                RaisePropertyChanged(() => IsEditBeerViewVisible);
            }
        }
        
        private bool _isListBeerViewVisible;
        public bool IsListBeerViewVisible
        {
            get { return _isListBeerViewVisible; }
            set
            {
                _isListBeerViewVisible = value;
                RaisePropertyChanged(() => IsListBeerViewVisible);
            }
        }

        #endregion


        #region Constructors

        public CollectionModuleViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                EventAggregator.GetEvent<ShowBeerEditRegionEvent>().Subscribe(ShowBeerEditRegion);
                EventAggregator.GetEvent<ShowBeerListRegionEvent>().Subscribe(ShowBeerListRegion);
            }
        }

        #endregion


        #region Commands methods

        public void ShowBeerEditRegion(bool obj)
        {
            IsEditBeerViewVisible = obj;
        }

        public void ShowBeerListRegion(bool obj)
        {
            IsListBeerViewVisible = obj;
        }

        #endregion
    }
}
