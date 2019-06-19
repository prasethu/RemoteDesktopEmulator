using System;
using System.Globalization;
using System.Windows.Data;

namespace RemoteDesktopEmulator
{
    internal class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool input = System.Convert.ToBoolean(value);
            return !input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool input = System.Convert.ToBoolean(value);
            return !input;
        }
    }
}
