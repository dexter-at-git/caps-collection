using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;

namespace CapsCollection.Silverlight.Infrastructure.Models
{
    public class BeerWithImages : BeerDto
    {
        public BeerWithImages()
        {
            BottleImage = new BeerImage(BeerImageType.Bottle);
            CapImage = new BeerImage(BeerImageType.Cap);
            LabelImage = new BeerImage(BeerImageType.Label);
        }

        public BeerWithImages(int beerId)
        {
            BeerId = beerId;
            BottleImage = new BeerImage(BeerImageType.Bottle, beerId);
            CapImage = new BeerImage(BeerImageType.Cap, beerId);
            LabelImage = new BeerImage(BeerImageType.Label, beerId);
        }
        
        BeerImage _bottleImage;
        public BeerImage BottleImage
        {
            get { return _bottleImage; }
            set
            {
                _bottleImage = value;
                RaisePropertyChanged("BottleImage");
            }
        }
        
        BeerImage _capImage;
        public BeerImage CapImage
        {
            get { return _capImage; }
            set
            {
                _capImage = value;
                RaisePropertyChanged("CapImage");
            }
        }
        
        private BeerImage _labelImage;
        public BeerImage LabelImage
        {
            get { return _labelImage; }
            set
            {
                _labelImage = value;
                RaisePropertyChanged("LabelImage");
            }
        }
    }
}
