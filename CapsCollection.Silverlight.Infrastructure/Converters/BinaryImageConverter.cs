using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CapsCollection.Silverlight.Infrastructure.Converters
{
    public class BinaryImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] buffer = value as byte[];
            if (buffer != null && buffer.Length > 0)
            {
                BitmapImage bmpImage = new BitmapImage();
                bmpImage.SetSource(new MemoryStream(buffer));

                return bmpImage;
            }

            BitmapImage img = new BitmapImage();

            string fileName = String.Empty;
            if (parameter != null)
            {
                switch (parameter.ToString())
                {
                    case "Flag":
                        fileName = "EmptyFlag";
                        break;
                    case "Bottle":
                        fileName = "EmptyBottle";
                        break;
                    case "Cap":
                        fileName = "EmptyCap";
                        break;
                    case "Label":
                        fileName = "EmptyLabel";
                        break;
                }

                var stream = System.Windows.Application.GetResourceStream(new Uri(String.Format("/CapsCollection.Silverlight.Infrastructure;component/Resources/Images/{0}.png", fileName), UriKind.Relative)).Stream;
                img.SetSource(stream);
            }
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
