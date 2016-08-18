using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.UI.Modules.BulkLoad.ViewModels;
using System.Windows.Controls;

namespace CapsCollection.Desktop.UI.Modules.BulkLoad.Views
{
    public partial class BeerLoadView : UserControl, IBeerLoadView
    {
        public BeerLoadView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get { return (IBeerLoadViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
