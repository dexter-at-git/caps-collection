using CapsCollection.Silverlight.UI.Shell.ViewModels;

namespace CapsCollection.Silverlight.UI.Shell.Views
{
    public partial class AuthenticationModule
    {
        private readonly AuthenticationModuleViewModel _viewModel;

        public AuthenticationModule()
        {
            InitializeComponent();
            
            _viewModel = new AuthenticationModuleViewModel();
            DataContext = _viewModel;
        }
    }
}