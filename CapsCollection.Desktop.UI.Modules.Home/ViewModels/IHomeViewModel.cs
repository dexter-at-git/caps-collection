using CapsCollection.Desktop.Infrastructure.Interfaces;
using Prism.Commands;

namespace CapsCollection.Desktop.UI.Modules.Home.ViewModels
{
    public interface IHomeViewModel : IViewModel
    {
        DelegateCommand ProcessImagesCommand { get; }
        void InitialzeTestData();
        string BottlesLookupPath { get; set; }
    }
}
