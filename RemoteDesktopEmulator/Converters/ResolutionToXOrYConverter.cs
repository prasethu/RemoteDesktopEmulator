using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemoteDesktopEmulator
{
	public class ResolutionToXOrYConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return string.Empty;
			}

			int index = parameter.ToString().Equals("X", StringComparison.InvariantCultureIgnoreCase) ? 0 : 1;
			return value.ToString().Split('x')[index].Trim();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
