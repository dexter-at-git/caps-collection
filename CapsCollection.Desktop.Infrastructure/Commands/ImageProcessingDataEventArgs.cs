using System.Collections.Generic;
using CapsCollection.Desktop.Infrastructure.Models;

namespace CapsCollection.Desktop.Infrastructure.Commands
{
    public class ImageProcessingDataEventArgs
    {
        public ImageProcessingDataEventArgs()
        {
            CombinedImages = new Dictionary<int, List<ImageData>>();
        }

        public Dictionary<int, List<ImageData>> CombinedImages { get; set; }

        public bool CheckUpdates { get; set; }
    }
}