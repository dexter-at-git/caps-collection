using CapsCollection.Silverlight.UI.Modules.Collection.Views;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;

namespace CapsCollection.Silverlight.UI.Modules.Collection
{
    [ModuleExport(typeof(CollectionModule))]
    public class CollectionModule : IModule
    {
        [Import]
        public IRegionManager RegionManager { get; set; }

        public void Initialize()
        {
            var countriesView = new BeerCountriesListView();
            RegionManager.AddToRegion("BeerCountriesContent", countriesView);

            var beersView = new BeerListView();
            RegionManager.AddToRegion("BeerListContent", beersView);

            var bottomMenuView = new BottomMenuView();
            RegionManager.AddToRegion("CollectionBottomMenuContent", bottomMenuView);
        }
    }
}
