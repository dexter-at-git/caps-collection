using CapsCollection.Silverlight.UI.Modules.Brewery.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.Brewery.Views
{
    public partial class BreweryListView
    {
        private readonly BreweryListViewModel _viewModel;

        public BreweryListView()
        {
            InitializeComponent();

            _viewModel = new BreweryListViewModel();
            DataContext = _viewModel;
        }
    }
}
