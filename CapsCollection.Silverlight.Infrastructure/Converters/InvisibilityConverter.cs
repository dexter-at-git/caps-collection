using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CapsCollection.Silverlight.Infrastructure.Converters
{
    public class InvisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }

            if (value is bool)
            {
                bool invisible = (bool)value;
                if (invisible)
                {
                    return Visibility.Collapsed;
                }
            }
            else if (value is int)
            {
                // any positive number makes it invisible too
                if ((int)value > 0) return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
