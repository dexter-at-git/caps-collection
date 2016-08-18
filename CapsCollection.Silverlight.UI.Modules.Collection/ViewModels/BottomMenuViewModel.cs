using CapsCollection.Silverlight.Infrastructure.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using CapsCollection.Silverlight.Infrastructure.Events;

namespace CapsCollection.Silverlight.UI.Modules.Collection.ViewModels
{
    public class BottomMenuViewModel : ViewModelBase
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion


        #region Commands and Events

        public DelegateCommand ReloadBeerCountriesCommand { get; private set; }
        public DelegateCommand AddBeerCommand { get; private set; }

        #endregion


        #region Constructors

        public BottomMenuViewModel()
        {
            // Commands.
            AddBeerCommand = new DelegateCommand(OnAddBeer);
            ReloadBeerCountriesCommand = new DelegateCommand(OnReload);

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);
            }
        }

        #endregion


        #region Commands methods

        private void OnAddBeer()
        {
            var uriQuery = new UriQuery();

            var uri = new Uri("BeerEditView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        private void OnReload()
        {
            EventAggregator.GetEvent<BeerCountriesReloadEvent>().Publish(true);
            EventAggregator.GetEvent<ShowBeerListRegionEvent>().Publish(false);
            EventAggregator.GetEvent<ShowBeerEditRegionEvent>().Publish(false);
        }

        #endregion
    }
}
