using CapsCollection.Silverlight.UI.Shell.ViewModels;

namespace CapsCollection.Silverlight.UI.Shell.Views
{
    public partial class CollectionModule
    {
        private readonly CollectionModuleViewModel _viewModel;

        public CollectionModule()
        {
            InitializeComponent();

            _viewModel = new CollectionModuleViewModel();
            this.DataContext = _viewModel;
        }
    }
}
