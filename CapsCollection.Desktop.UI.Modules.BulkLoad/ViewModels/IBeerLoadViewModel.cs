using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.Infrastructure.Interfaces;
using Prism.Commands;

namespace CapsCollection.Desktop.UI.Modules.BulkLoad.ViewModels
{
    public interface IBeerLoadViewModel : IViewModel
    {
        BeerLoadViewModel PrepareViewModel(BeerLoadDataEventArgs imageList);
        DelegateCommand<BeerLoadViewModel> SaveBeerCommand { get; }
    }
}
