using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CapsCollection.Silverlight.Infrastructure.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            if (value is bool)
            {
                bool visibile = (bool)value;
                if (visibile)
                {
                    return Visibility.Visible;
                }
            }
            else if (value is int)
            {
                if ((int)value > 0) return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
