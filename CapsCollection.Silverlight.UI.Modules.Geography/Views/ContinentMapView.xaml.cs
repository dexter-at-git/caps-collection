using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace CapsCollection.Silverlight.UI.Modules.Geography.Views
{
    [Export("ContinentMapView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ContinentMapView : UserControl
    {
        public ContinentMapView()
        {
            InitializeComponent();
        }
    }
}
