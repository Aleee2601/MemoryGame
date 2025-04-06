using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MemoryGame.Converters
{
    public class ColorToSolidBrushConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return Brushes.Gray;

            try
            {
                string? colorString = value.ToString();
                
                if (colorString == null)
                    return Brushes.Gray;
                
                // Check if it's a hex color code
                if (colorString.StartsWith("#") && (colorString.Length == 7 || colorString.Length == 9))
                {
                    Color color = (Color)ColorConverter.ConvertFromString(colorString);
                    return new SolidColorBrush(color);
                }
            }
            catch (Exception)
            {
                // Return a default brush if there's an error
                return Brushes.Gray;
            }
            
            return Brushes.Gray;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
