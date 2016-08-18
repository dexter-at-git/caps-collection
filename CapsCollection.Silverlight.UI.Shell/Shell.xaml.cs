using System;
using CapsCollection.Silverlight.UI.Shell.Views;
using Microsoft.Practices.Prism.Modularity;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CapsCollection.Silverlight.UI.Shell
{
    [Export]
    public partial class Shell
    {
        [Import(AllowRecomposition = false)]
        public IModuleManager ModuleManager;

        [ImportingConstructor]
        public Shell(IModuleManager moduleManager)
        {
            ModuleManager = moduleManager;

            InitializeComponent();

            // Create a link to hosting site.
            var xapHostingUriString = Application.Current.Host.Source.AbsoluteUri;
            var xapHostingUri = new Uri(xapHostingUriString);
            var host = xapHostingUri.Host;
            var port = xapHostingUri.Port;
            var schema = xapHostingUri.Scheme;
            var hostSite = String.Format("{0}://{1}:{2}", schema, host, port);
            HomeSiteHyperlink.NavigateUri = new Uri(hostSite);
        }

        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            LoadModule(e.Uri.ToString());

            foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (hb.NavigateUri.ToString().Equals(e.Uri.ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        private void LoadModule(string uri)
        {
            // if link requires a module then load it
            if (Navigation.ModuleMapper.ModuleMaps.ContainsKey(uri))
            {
                ModuleManager.LoadModule(Navigation.ModuleMapper.ModuleMaps[uri]);
            }
        }

        // If an error occurs during navigation, show an error window
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }

        private string GetParam(string p)
        {
            if (App.Current.Resources[p] != null)
                return Application.Current.Resources[p].ToString();
            else
                return string.Empty;
        }
    }
}