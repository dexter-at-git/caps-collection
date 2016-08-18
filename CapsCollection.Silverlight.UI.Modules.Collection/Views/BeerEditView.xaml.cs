using System.ComponentModel.Composition;
using CapsCollection.Silverlight.UI.Modules.Collection.Validators;
using CapsCollection.Silverlight.UI.Modules.Collection.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.Collection.Views
{
    [Export("BeerEditView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class BeerEditView
    {
        private readonly BeerEditViewModel _viewModel;

        public BeerEditView()
        {
            InitializeComponent();

            _viewModel = new BeerEditViewModel(BeerModelValidator.Create());
            DataContext = _viewModel;
        }
    }
}
