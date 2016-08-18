using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.Infrastructure.Interfaces;
using Prism.Commands;

namespace CapsCollection.Desktop.UI.Modules.BulkUpdate.ViewModels
{
    public interface IBeerUpdateViewModel : IViewModel
    {
        BeerUpdateViewModel PrepareViewModel(BeerLoadDataEventArgs imageList);
        DelegateCommand<BeerUpdateViewModel> SaveBeerCommand { get; }
    }
}
