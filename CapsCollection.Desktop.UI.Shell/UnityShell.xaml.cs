using CapsCollection.Desktop.UI.Shell.ViewModels;
using Microsoft.Practices.Unity;

namespace CapsCollection.Desktop.UI.Shell
{
    public partial class UnityShell
    {
        [Dependency]
        public UnityShellViewModel UnityShellViewModel
        {
            get { return this.DataContext as UnityShellViewModel; }
            set { this.DataContext = value; }
        }

        public UnityShell()
        {
            InitializeComponent();
        }
    }
}
