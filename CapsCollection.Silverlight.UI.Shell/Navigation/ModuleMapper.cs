using System.Collections.Generic;

namespace CapsCollection.Silverlight.UI.Shell.Navigation
{
    public static class ModuleMapper
    {
        public static Dictionary<string, string> ModuleMaps { get; set; }

        static ModuleMapper()
        {
            // if any navigation pages have prism regions then put the map to the relevant
            // module here.  The module will then be dynamically loaded when necessary.
            ModuleMaps = new Dictionary<string, string>();
            ModuleMaps.Add("/GeographyModule", "GeographyModule");
            ModuleMaps.Add("/BreweryModule", "BreweryModule");
            ModuleMaps.Add("/BeerStyleModule", "BeerStyleModule");
            ModuleMaps.Add("/CollectionModule", "CollectionModule");
            ModuleMaps.Add("/AuthenticationModule", "AuthenticationModule");
        }
    }
}
