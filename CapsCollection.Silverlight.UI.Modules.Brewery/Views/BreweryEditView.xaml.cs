using CapsCollection.Silverlight.UI.Modules.Brewery.Validators;
using CapsCollection.Silverlight.UI.Modules.Brewery.ViewModels;
using System.ComponentModel.Composition;

namespace CapsCollection.Silverlight.UI.Modules.Brewery.Views
{
    [Export("BreweryEditView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class BreweryEditView
    {
        private readonly BreweryEditViewModel _viewModel;

        public BreweryEditView()
        {
            InitializeComponent();

            _viewModel = new BreweryEditViewModel(BreweryModelValidator.Create());
            DataContext = _viewModel;
        }
    }
}
