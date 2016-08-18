using System.ComponentModel.Composition;
using System.Windows.Controls;
using CapsCollection.Silverlight.UI.Modules.BeerStyle.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.BeerStyle.Views
{
    [Export("BeerStyleListView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class BeerStyleListView : UserControl
    {
        private readonly BeerStyleListViewModel _viewModel;

        public BeerStyleListView()
        {
            InitializeComponent();

            _viewModel = new BeerStyleListViewModel();
            this.DataContext = _viewModel;
        }
    }
}
