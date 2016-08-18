using System;
using System.Windows;
using System.Windows.Data;

namespace CapsCollection.Desktop.Infrastructure.Converters
{
    public class InvisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
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

            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
