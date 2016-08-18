using CapsCollection.Desktop.Infrastructure.Models;
using Prism.Events;

namespace CapsCollection.Desktop.Infrastructure.Commands
{
    public class BeerLoadingStatusEvent : PubSubEvent<string> { }
    public class BeerErrorEvent : PubSubEvent<BeerErrorEventArgs> { }
    public class BeerLoadingInProgressEvent : PubSubEvent<LoadingProgress> { }
    public class ImagesProcessingEvent : PubSubEvent<ImageProcessingDataEventArgs> { }
    public class BusyEvent : PubSubEvent<bool> { }
    public class ImageToLoadEvent : PubSubEvent<BeerLoadDataEventArgs> { }
    public class ImageToUpdateEvent : PubSubEvent<BeerLoadDataEventArgs> { }
    public class BeerSavingEvent : PubSubEvent<BeerSavingDataEventArgs> { }
    public class BeerSavedEvent : PubSubEvent<BeerSavedDataEventArgs> { }
}
