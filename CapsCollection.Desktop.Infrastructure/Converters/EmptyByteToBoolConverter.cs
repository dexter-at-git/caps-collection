﻿using System;
using System.Windows.Data;

namespace CapsCollection.Desktop.Infrastructure.Converters
{
    public class EmptyByteToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if ((value is byte[]) && (((byte[])value).Length != 0))
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
