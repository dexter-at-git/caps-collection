using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Views
{
    [Export("CityListView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CityListView : UserControl
    {
        public CityListView()
        {
            InitializeComponent();
        }
    }
}
