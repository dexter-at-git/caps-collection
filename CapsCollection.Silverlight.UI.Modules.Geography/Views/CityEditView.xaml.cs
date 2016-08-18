using CapsCollection.Silverlight.UI.Modules.Geography.Validators;
using CapsCollection.Silverlight.UI.Modules.Geography.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Views
{
    [Export("CityEditView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CityEditView : UserControl
    {
        private CityEditViewModel vm;

        public CityEditView()
        {
            InitializeComponent();

            vm = new CityEditViewModel(CityModelValidator.Create());
            this.DataContext = vm;
        }
    }
}
