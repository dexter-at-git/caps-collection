using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.UI.Modules.BulkLoad.ViewModels;
using System.Windows.Controls;

namespace CapsCollection.Desktop.UI.Modules.BulkLoad.Views
{
    public partial class BulkLoadView : UserControl, IBulkLoadView
    {
        public BulkLoadView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get { return (IBulkLoadViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
