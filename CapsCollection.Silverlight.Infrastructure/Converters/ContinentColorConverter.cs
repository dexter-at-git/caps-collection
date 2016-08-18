using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CapsCollection.Silverlight.Infrastructure.Converters
{
    public class ContinentColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((SolidColorBrush)value).Color == ((SolidColorBrush)parameter).Color)
            {
                return (SolidColorBrush)value;
            }
            return new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
