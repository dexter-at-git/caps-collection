using System;
using System.Windows;
using CapsCollection.Desktop.UI.Shell.Bootstrapper;

namespace CapsCollection.Desktop.UI.Shell
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;

            BootstrapperUnity bootstrapper = new BootstrapperUnity();
            bootstrapper.Run();
        }
        
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}{1}{2}", e.Exception.Message, Environment.NewLine, e.Exception.StackTrace);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
