using System.ComponentModel.Composition;
using System.Windows.Controls;
using CapsCollection.Silverlight.UI.Modules.BeerStyle.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.BeerStyle.Views
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
