using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Views
{
    [Export("CountryListView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CountryListView : UserControl
    {
        public CountryListView()
        {
            InitializeComponent();
        }
    }
}
