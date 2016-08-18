using CapsCollection.Silverlight.Infrastructure.ViewModels;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel;
using System.ComponentModel.Composition;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Authentication;

namespace CapsCollection.Silverlight.UI.Shell.ViewModels
{
    public class AuthenticationModuleViewModel : ViewModelBase
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }

        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion
        

        #region Constructors

        public AuthenticationModuleViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                EventAggregator.GetEvent<IsAuthenticatedEvent>().Subscribe(CheckAuthentication);
            }
        }

        #endregion


        #region Commands methods

        public void CheckAuthentication(AuthenticationData authenticationData)
        {
            IsAuthenticated = authenticationData.IsAuthenticated;
        }

        #endregion
    }
}
