using CapsCollection.Silverlight.UI.Modules.Authentication.Views;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;

namespace CapsCollection.Silverlight.UI.Modules.Authentication
{
    [ModuleExport(typeof(AuthenticationModule))]
    public class AuthenticationModule : IModule
    {
        [Import]
        public IRegionManager RegionManager { get; set; }
        
        public void Initialize()
        {
            var loginView = new LoginView();
            RegionManager.AddToRegion("LoginContent", loginView);

            var userInfoView = new UserInfoView();
            RegionManager.AddToRegion("UserInfoContent", userInfoView);
        }
    }
}
