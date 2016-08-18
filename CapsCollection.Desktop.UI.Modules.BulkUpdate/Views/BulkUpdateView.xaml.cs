using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.ViewModels;
using System.Windows.Controls;

namespace CapsCollection.Desktop.UI.Modules.BulkUpdate.Views
{
    public partial class BulkUpdateView : UserControl, IBulkUpdateView
    {
        public BulkUpdateView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get { return (IBulkUpdateViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
