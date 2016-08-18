using System.Windows.Controls;
using CapsCollection.Silverlight.UI.Modules.Brewery.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.Brewery.Views
{
    public partial class BreweryListFilterView : UserControl
    {
        private readonly BreweryListFilterViewModel _viewModel;

        public BreweryListFilterView()
        {
            InitializeComponent();

            _viewModel = new BreweryListFilterViewModel();
            DataContext = _viewModel;
        }
    }
}
