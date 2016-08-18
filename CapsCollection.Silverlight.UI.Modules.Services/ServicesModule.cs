using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace CapsCollection.Silverlight.UI.Modules.Services
{
    [ModuleExport(typeof(ServicesModule))]
    public class ServicesModule : IModule
    {
        public void Initialize()
        {
        }
    }
}
