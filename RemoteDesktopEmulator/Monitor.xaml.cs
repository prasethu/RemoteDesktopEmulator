using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemoteDesktopEmulator
{
	/// <summary>
	/// Interaction logic for Monitor.xaml
	/// </summary>
	public partial class Monitor : UserControl
	{
		public Monitor()
		{
			InitializeComponent();

			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			DpiScalingFactorsSelection.SelectedIndex = 0;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			MonitorViewModel model = this.DataContext as MonitorViewModel;
			Button button = sender as Button;

			int resX = int.Parse(button.Content.ToString().Split('x')[0]);
			int resY = int.Parse(button.Content.ToString().Split('x')[1]);

			model.ResolutionX = resX;
			model.ResolutionY = resY;
		}
	}
}
