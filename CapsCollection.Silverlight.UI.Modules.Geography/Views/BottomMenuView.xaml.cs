using CapsCollection.Silverlight.UI.Modules.Geography.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Views
{
    [Export("BottomMenuView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class BottomMenuView : UserControl
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
