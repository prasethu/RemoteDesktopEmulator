using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
		[DllImport("user32.dll")]
		private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		private static extern bool IsIconic(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		const int swRestore = 9;

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
			Process[] allProcesses = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (allProcesses.Length > 1)
			{
				foreach (Process p in allProcesses)
				{
					if (p.Id != Process.GetCurrentProcess().Id)
					{
						IntPtr hWnd = allProcesses[1].MainWindowHandle;

						if (IsIconic(hWnd))
						{
							ShowWindowAsync(hWnd, swRestore);
						}

						// bring it to the foreground
						SetForegroundWindow(hWnd);
					}
				}

				this.Close();
			}
			else
			{
				if (!UacHelper.IsProcessElevated)
				{
					RestartAsAdmin();
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

		private void Window_Closed(object sender, EventArgs e)
		{
			RegistryUtils.CleanupRegistry();
		}
	}
}
