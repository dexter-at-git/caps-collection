using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace CapsCollection.Silverlight.UI.Modules.BeerStyle.ViewModels
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

        public DelegateCommand AddBeerStyleCommand { get; private set; }

        #endregion


        #region Properties and Members


        #endregion


        #region Constructors

        public BottomMenuViewModel()
        {
            // Commands.
            AddBeerStyleCommand = new DelegateCommand(OnAddBeerStyle);

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);
            }
        }

        #endregion


        #region Buttons click command methods

        private void OnAddBeerStyle()
        {
            var uri = new Uri("BeerStyleEditView", UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        #endregion
    }
}
