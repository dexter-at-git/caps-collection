using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.ViewModels;
using System.Windows.Controls;

namespace CapsCollection.Desktop.UI.Modules.BulkUpdate.Views
{
    public partial class BeerUpdateView : UserControl, IBeerUpdateView
    {
        public BeerUpdateView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get { return (IBeerUpdateViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
