using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.UI.Modules.StatusBar.ViewModels;
using System.Windows.Controls;

namespace CapsCollection.Desktop.UI.Modules.StatusBar.Views
{
    public partial class StatusBarView : UserControl, IStatusBarView
    {
        public StatusBarView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get { return (IStatusBarViewModel)DataContext; }
            set { DataContext = value; }
        }
    }

}
