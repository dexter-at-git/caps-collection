using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.UI.Modules.Settings.ViewModels;
using System.Windows.Controls;

namespace CapsCollection.Desktop.UI.Modules.Settings.Views
{
    public partial class SettingsView : UserControl, ISettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get { return (ISettingsViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
