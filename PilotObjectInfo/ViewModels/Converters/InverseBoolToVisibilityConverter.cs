using System;
using System.Windows;
using System.Windows.Data;

namespace PilotObjectInfo.ViewModels.Converters
{
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility visibilityValue)
            {
                return visibilityValue != Visibility.Visible;
            }
            return true;
        }
    }
}
