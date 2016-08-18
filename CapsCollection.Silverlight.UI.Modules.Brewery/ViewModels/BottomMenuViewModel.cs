using CapsCollection.Silverlight.Infrastructure.ViewModels;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using CapsCollection.Silverlight.Infrastructure.Events;

namespace CapsCollection.Silverlight.UI.Modules.Brewery.ViewModels
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

        public DelegateCommand ReloadBreweriesCommand { get; private set; }
        public DelegateCommand AddBreweryCommand { get; private set; }

        #endregion


        #region Constructors

        public BottomMenuViewModel()
        {
            // Commands.
            AddBreweryCommand = new DelegateCommand(OnAddBrewery);
            ReloadBreweriesCommand = new DelegateCommand(OnReload);

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);
            }
        }

        #endregion


        #region Commands methods

        private void OnAddBrewery()
        {
            var uri = new Uri("BreweryEditView", UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        private void OnReload()
        {
            EventAggregator.GetEvent<BreweriesReloadEvent>().Publish(true);
            EventAggregator.GetEvent<ShowBreweriesListRegionEvent>().Publish(false);
            EventAggregator.GetEvent<ShowBreweriesEditRegionEvent>().Publish(false);
        }

        #endregion
    }
}
