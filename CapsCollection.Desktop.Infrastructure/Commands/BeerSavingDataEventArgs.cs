using System;
using System.Collections.Generic;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.Infrastructure.Models;

namespace CapsCollection.Desktop.Infrastructure.Commands
{
    public class BeerSavingDataEventArgs
    {
        public BeerSavingDataEventArgs()
        {
            ImageList = new List<BeerImage>();
        }
        public BeerDto Beer { get; set; }
        public BeerImage BottleImage { get; set; }
        public BeerImage CapImage { get; set; }
        public BeerImage LabelImage { get; set; }
        public List<BeerImage> ImageList { get; set; }
        public Guid BeerTempId { get; set; }
    }
}
