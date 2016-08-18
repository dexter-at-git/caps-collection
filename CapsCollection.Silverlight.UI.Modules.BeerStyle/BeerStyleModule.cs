using System.ComponentModel.Composition;
using CapsCollection.Silverlight.UI.Modules.BeerStyle.Views;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace CapsCollection.Silverlight.UI.Modules.BeerStyle
{
    [ModuleExport(typeof(BeerStyleModule))]
    public class BeerStyleModule : IModule
    {
        [Import]
        public IRegionManager RegionManager { get; set; }

        public void Initialize()
        {
            var bottomMenuView = new BottomMenuView();
            RegionManager.AddToRegion("BeerStyleBottomMenuContent", bottomMenuView);

            var beerStyleView = new BeerStyleListView();
            RegionManager.AddToRegion("BeerStyleContent", beerStyleView);
        }
    }
}
