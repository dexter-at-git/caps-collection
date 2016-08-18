using CapsCollection.Silverlight.UI.Modules.Brewery.Views;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;

namespace CapsCollection.Silverlight.UI.Modules.Brewery
{
    [ModuleExport(typeof(BreweryModule))]
    public class BreweryModule : IModule
    {
        [Import]
        public IRegionManager RegionManager { get; set; }

        public void Initialize()
        {
            var breweryFilterView = new BreweryListFilterView();
            RegionManager.AddToRegion("BreweryFilterContent", breweryFilterView);

            var breweryView = new BreweryListView();
            RegionManager.AddToRegion("BreweryContent", breweryView);

            var bottomMenuView = new BottomMenuView();
            RegionManager.AddToRegion("BreweryBottomMenuContent", bottomMenuView);
        }
    }
}
