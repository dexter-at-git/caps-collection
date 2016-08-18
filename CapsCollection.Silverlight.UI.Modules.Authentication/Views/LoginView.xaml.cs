using System.ComponentModel.Composition;
using CapsCollection.Silverlight.UI.Modules.Authentication.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.Authentication.Views
{
    [Export("LoginView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoginView
    {
        private readonly LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();

            _viewModel = new LoginViewModel();
            DataContext = _viewModel;
        }
    }
}
