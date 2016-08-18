using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace CapsCollection.Silverlight.Infrastructure.Converters
{
    public class BlankComboBoxItemConverter : IValueConverter
    {
        private const string NoneString = "[None]";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable set = (IEnumerable)value;
            IList list = new List<object>();

            // use the BlankNameEntry as the default
            if (parameter == null)
            {
                list.Add(new BlankNameEntry { Id = null, EnglishContinentName = NoneString });
            }
                // look at the parameter variable and get the class name and property name from it
            else
            {
                string[] splitParams = ((string)parameter).Split(',');

                if (splitParams.Length != 2)
                {
                    throw new InvalidOperationException("Invalid string passed as converter parameter. Must be in the form of <string>,<string>.");
                }

                string className = splitParams[0];
                string propertyName = splitParams[1];

                Type classType = Type.GetType(string.Format("EventBoard.Converters.Blank.{0}", className));

                if (classType == null)
                {
                    throw new InvalidOperationException(string.Format("EventBoard.Converters.Blank.{0} does not exist.", className));
                }

                object classObject = Activator.CreateInstance(classType);
                PropertyInfo property = classType.GetProperty(propertyName);

                property.SetValue(classObject, NoneString, null);

                list.Add(classObject);
            }

            foreach (object entity in set)
            {
                list.Add(entity);
            }

            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    
    public abstract class BlankEntry
    {
        public int? Id { get; set; }
    }

    public class BlankNameEntry : BlankEntry
    {
        public string EnglishContinentName { get; set; }
    }
}