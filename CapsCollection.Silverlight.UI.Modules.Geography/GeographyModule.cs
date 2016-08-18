using CapsCollection.Silverlight.UI.Modules.Geography.Views;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;

namespace CapsCollection.Silverlight.UI.Modules.Geography
{
    [ModuleExport(typeof(GeographyModule))]
    public class GeographyModule : IModule
    {
        [Import]
        public IRegionManager RegionManager { get; set; }

        public void Initialize()
        {
            RegionManager.RequestNavigate("PopupRegionContent", "ContinentMapView");

            var bottomMenuView = new BottomMenuView();
            RegionManager.AddToRegion("GeographyBottomMenuContent", bottomMenuView);

            var countryView = new CountryListView();
            RegionManager.AddToRegion("CountryContent", countryView);

            var regionsView = new RegionListView();
            RegionManager.AddToRegion("RegionContent", regionsView);

            var citiesView = new CityListView();
            RegionManager.AddToRegion("CityContent", citiesView);
        }
    }
}
