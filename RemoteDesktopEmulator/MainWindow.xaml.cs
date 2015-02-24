using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			this.DataContext = new MainWindowViewModel();

			Loaded += OnLoaded;
		}
		
		private MainWindowViewModel Model
		{
			get
			{
				return this.DataContext as MainWindowViewModel;
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (!UacHelper.IsProcessElevated)
			{
				MessageBoxResult result = MessageBox.Show(string.Format(
					@"I need to be run as an administrator to show my capabilities.{0}{1}Restart as admin?", Environment.NewLine, Environment.NewLine),
					"Remote Desktop Emulator", MessageBoxButton.YesNo);
				
				switch (result)
				{
					case MessageBoxResult.Yes:
						RestartAsAdmin();

						break;

					case MessageBoxResult.No:
						this.Close();

						break;
				}
			}
		}

		private void RestartAsAdmin()
		{
			Process currentProcess = Process.GetCurrentProcess();
			currentProcess.StartInfo.Verb = "runas";
			currentProcess.StartInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
			currentProcess.Start();

			this.Close();
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox combo = sender as ComboBox;
			int monitors = (int)combo.SelectedValue;

			this.Model.SetNumberOfMonitors(monitors);
		}
	}
}
