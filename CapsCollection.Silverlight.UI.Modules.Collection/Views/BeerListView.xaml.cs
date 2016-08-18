using CapsCollection.Silverlight.UI.Modules.Collection.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.Collection.Views
{
    public partial class BeerListView
    {
        private readonly BeerListViewModel _viewModel;

        public BeerListView()
        {
            InitializeComponent();

            _viewModel = new BeerListViewModel();
            DataContext = _viewModel;
        }
    }
}
