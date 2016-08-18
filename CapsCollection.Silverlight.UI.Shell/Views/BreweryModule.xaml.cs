using CapsCollection.Silverlight.Infrastructure.Commands;
using CapsCollection.Silverlight.UI.Shell.ViewModels;
using System.Windows.Input;

namespace CapsCollection.Silverlight.UI.Shell.Views
{
    public partial class BreweryModule
    {
        public ICommand SaveAllCommand { get { return Commands.SaveAllCommand; } }

        private readonly BreweryModuleViewModel _viewModel;

        public BreweryModule()
        {
            InitializeComponent();

            _viewModel = new BreweryModuleViewModel();
            this.DataContext = _viewModel;
        }
    }
}
