using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MemoryGame.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool invert = parameter != null && parameter.ToString()?.ToLower() == "invert";
            bool boolValue = value is bool b && b;
            
            if (invert)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool invert = parameter != null && parameter.ToString()?.ToLower() == "invert";
            bool isVisible = value is Visibility visibility && visibility == Visibility.Visible;
            
            if (invert)
            {
                return !isVisible;
            }
            
            return isVisible;
        }
    }
}
