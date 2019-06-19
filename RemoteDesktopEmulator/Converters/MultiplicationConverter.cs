using System;
using System.Globalization;
using System.Windows.Data;

namespace RemoteDesktopEmulator
{
    internal class MultiplicationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double multiplicand = System.Convert.ToDouble(value);
            double multiplicator = System.Convert.ToDouble(parameter);

            return multiplicand * multiplicator;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double multiplicand = System.Convert.ToDouble(value);
            double multiplicator = System.Convert.ToDouble(parameter);

            return multiplicand / multiplicator;
        }
    }
}
