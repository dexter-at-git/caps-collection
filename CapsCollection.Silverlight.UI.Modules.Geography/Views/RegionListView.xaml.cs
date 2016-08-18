using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Views
{
    [Export("RegionListView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RegionListView : UserControl
    {
        public RegionListView()
        {
            InitializeComponent();
        }
    }
}
