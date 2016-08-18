using CapsCollection.Silverlight.UI.Modules.Geography.Validators;
using CapsCollection.Silverlight.UI.Modules.Geography.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Views
{
    [Export("RegionEditView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RegionEditView : UserControl
    {
        private RegionEditViewModel vm;

        public RegionEditView()
        {
            InitializeComponent();

            vm = new RegionEditViewModel(RegionModelValidator.Create());
            this.DataContext = vm;
        }
    }
}
