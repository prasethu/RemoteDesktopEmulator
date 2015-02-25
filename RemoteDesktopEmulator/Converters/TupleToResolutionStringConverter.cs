using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemoteDesktopEmulator
{
	public class TupleToResolutionStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var vals = value as ObservableCollection<Resolution>;

			ObservableCollection<string> resolutionString = new ObservableCollection<string>();
			
			foreach (Resolution resolution in vals)
			{
				// resolutionString.Add(resolution.Value.Item1 + " x " + resolution.Value.Item2);
			}

			return resolutionString;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
