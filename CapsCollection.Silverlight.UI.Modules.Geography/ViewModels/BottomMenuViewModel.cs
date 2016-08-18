using CapsCollection.Silverlight.Infrastructure.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace CapsCollection.Silverlight.UI.Modules.Geography.ViewModels
{
    public class BottomMenuViewModel : ViewModelBase
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion


        #region Commands
        public DelegateCommand ChooseContinentCommand { get; private set; }

        #endregion


        #region Constructors

        public BottomMenuViewModel()
        {
            // Commands.
            ChooseContinentCommand = new DelegateCommand(OnChooseContinent);

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);
            }
        }

        #endregion


        #region Buttons click command methods

        private void OnChooseContinent()
        {
            var uriQuery = new UriQuery();

            var uri = new Uri("ContinentMapView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        #endregion
    }
}
