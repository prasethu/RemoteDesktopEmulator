using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemoteDesktopEmulator
{
	class DimensionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int v = int.Parse(value.ToString());

			int maxValue;
			if (!int.TryParse(parameter.ToString(), out maxValue))
			{
				throw new ArgumentException("Invalid type", "maxValue");
			}

			return ((double)v / (double)maxValue) * 300;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
