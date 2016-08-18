using CapsCollection.Silverlight.UI.Modules.Brewery.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.Brewery.Views
{
    public partial class BottomMenuView
    {
        private readonly BottomMenuViewModel _viewModel;

        public BottomMenuView()
        {
            InitializeComponent();

            _viewModel = new BottomMenuViewModel();
            DataContext = _viewModel;
        }
    }
}
