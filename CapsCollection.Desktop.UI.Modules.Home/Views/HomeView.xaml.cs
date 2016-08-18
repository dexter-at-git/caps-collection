using System.Windows.Controls;
using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.UI.Modules.Home.ViewModels;

namespace CapsCollection.Desktop.UI.Modules.Home.Views
{
    public partial class HomeView : UserControl, IHomeView
    {
        public HomeView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get { return (IHomeViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
