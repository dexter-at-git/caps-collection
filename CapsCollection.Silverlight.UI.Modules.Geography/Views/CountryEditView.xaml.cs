using CapsCollection.Silverlight.UI.Modules.Geography.Validators;
using CapsCollection.Silverlight.UI.Modules.Geography.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Views
{
    [Export("CountryEditView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CountryEditView : UserControl
    {
        private CountryEditViewModel vm;

        public CountryEditView()
        {
            InitializeComponent();

            vm = new CountryEditViewModel(CountryModelValidator.Create());
            this.DataContext = vm;
        }
    }
}
