using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CapsCollection.Desktop.Infrastructure.Converters
{
    public class BinaryImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri uri = null;
            string fileName = "EmptyImage";
            if (parameter != null)
            {
                switch (parameter.ToString())
                {
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
            }

            uri = new Uri(string.Format("pack://application:,,,/CapsCollection.Desktop.Infrastructure;component/Resources/Images/{0}.png",
                      fileName), UriKind.Absolute);

            var image = new BitmapImage();
            
            if (value == null || ((byte[])value).Length == 0)
            {
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = uri;
                image.EndInit();
                image.Freeze();
            }
            else
            {
                try
                {
                    using (var mem = new MemoryStream((byte[])value))
                    {
                        mem.Position = 0;
                        //    mem.Seek(0, SeekOrigin.Begin);
                        image.BeginInit();
                        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = mem;
                        image.EndInit();
                        image.Freeze();
                    }
                }
                catch (Exception ex)
                {

                    image = new BitmapImage();
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri("pack://application:,,,/CapsCollection.Desktop.Infrastructure;component/Resources/Images/Error.png", UriKind.Absolute);
                    image.EndInit();
                    image.Freeze();
                    throw new Exception(ex.Message);
                }
            }

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
