using System.ComponentModel.Composition;
using CapsCollection.Silverlight.UI.Modules.Authentication.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.Authentication.Views
{
    [Export("UserInfoView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UserInfoView
    {
        private readonly UserInfoViewModel _viewModel;

        public UserInfoView()
        {
            InitializeComponent();

            _viewModel = new UserInfoViewModel();
            DataContext = _viewModel;
        }
    }
}
