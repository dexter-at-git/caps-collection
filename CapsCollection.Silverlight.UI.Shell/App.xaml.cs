using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CapsCollection.Silverlight.UI.Shell.Views;

namespace CapsCollection.Silverlight.UI.Shell
{
    public partial class App
    {
        public IDictionary<string, string> DeploymentConfigurations;

        public App()
        {
            Startup += Application_Startup;
            UnhandledException += Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DeploymentConfigurations = e.InitParams;

            // Get startup parameters.
            if (e.InitParams != null)
            {
                foreach (var item in e.InitParams)
                {
                    Resources.Add(item.Key, item.Value);
                }
            }

            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // a ChildWindow control.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                ChildWindow errorWin = new ErrorWindow(e.ExceptionObject);
                errorWin.Show();
            }
        }
    }
}