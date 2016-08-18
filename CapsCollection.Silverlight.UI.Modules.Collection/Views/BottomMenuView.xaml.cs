using CapsCollection.Silverlight.UI.Modules.Collection.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.Collection.Views
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
