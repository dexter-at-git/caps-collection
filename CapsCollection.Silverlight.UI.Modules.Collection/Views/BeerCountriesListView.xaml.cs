using CapsCollection.Silverlight.UI.Modules.Collection.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.Collection.Views
{
    public partial class BeerCountriesListView
    {
        private readonly BeerCountriesListViewModel _viewModel;

        public BeerCountriesListView()
        {
            InitializeComponent();

            _viewModel = new BeerCountriesListViewModel();
            DataContext = _viewModel;
        }
    }
}
