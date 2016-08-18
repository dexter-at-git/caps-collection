using System.ComponentModel.Composition;
using System.Windows.Controls;
using CapsCollection.Silverlight.UI.Modules.BeerStyle.Validators;
using CapsCollection.Silverlight.UI.Modules.BeerStyle.ViewModels;

namespace CapsCollection.Silverlight.UI.Modules.BeerStyle.Views
{
    [Export("BeerStyleEditView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class BeerStyleEditView : UserControl
    {
        private BeerStyleEditViewModel vm;

        public BeerStyleEditView()
        {
            InitializeComponent();

            vm = new BeerStyleEditViewModel(BeerStyleModelValidator.Create());
            this.DataContext = vm;
        }
    }
}
