using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RemoteDesktopEmulator
{
	internal class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel()
		{
			_connectCommand = new ConnectCommand();
			_resetCommand   = new ResetCommand();
		}

		ICommand ChangeCommand { get; set; }

		private ObservableCollection<int> _totalMonitors = new ObservableCollection<int>() { 1, 2, 3, 4 };
		public ObservableCollection<int> TotalMonitors
		{
			get { return _totalMonitors; }
		}

		private ObservableCollection<MonitorViewModel> _monitors = new ObservableCollection<MonitorViewModel>();

		public ObservableCollection<MonitorViewModel> Monitors
		{
			get { return _monitors; }
		}

		public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
		{
			var propertyChanged = PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion	INotifyPropertyChanged

		#region Commands

		private readonly ICommand _connectCommand;

		public ICommand ConnectCommand
		{
			get { return _connectCommand; }
		}

		private readonly ICommand _resetCommand;

		public ICommand ResetCommand
		{
			get { return _resetCommand; }
		}

		#endregion Commands

		#region Methods

		internal void SetNumberOfMonitors(int count)
		{
			if (count < 1 || count > 10)
			{
				throw new ArgumentException("Out of bounds", "count");
			}

			_monitors.Clear();

			for (int value = 0; value < count; value++)
			{
				_monitors.Add(new MonitorViewModel(value + 1, 1920, 1080, 100));
			}
		}

		#endregion Methods
	}

	public class ConnectCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			ObservableCollection<MonitorViewModel> monitors = parameter as ObservableCollection<MonitorViewModel>;

			RegistryUtils.CleanupRegistry();

			using (RegistryKey userKey = Registry.CurrentUser)
			using (RegistryKey terminalServerKey = userKey.OpenSubKey(@"Software\Microsoft\Terminal Server Client", writable: true))
			{
				terminalServerKey.SetValue("MonitorConfigSource", 1);
				terminalServerKey.CreateSubKey(@"Default\AddIns\RDPDR");

				int left   = 0x00000000;
                int top    = 0x00000000;
				int right  = 0x00000000;
				int bottom = 0x00000000;
				
                foreach (MonitorViewModel monitor in monitors)
				{
					right = left + monitor.ResolutionX - 1;
					bottom = monitor.ResolutionY - 1;

					using (RegistryKey monitorKey = terminalServerKey.CreateSubKey(string.Format(@"Monitors\{0}", monitor.Id - 1)))
					{
						monitorKey.SetValue("left", left);
						monitorKey.SetValue("top", top);
						monitorKey.SetValue("right", right);
						monitorKey.SetValue("bottom", bottom);
						monitorKey.SetValue("flags", (monitor.Id == 1) ? 0x00000001 : 0x00000000); /* Identifies the primary monitor*/
						monitorKey.SetValue("physicalWidth", monitor.ResolutionX / 5);
						monitorKey.SetValue("physicalHeight", monitor.ResolutionY / 5);
						monitorKey.SetValue("orientation", 0x00000000);
						monitorKey.SetValue("desktopScaleFactor", monitor.DpiScaling);
						monitorKey.SetValue("deviceScaleFactor", 0x000000b4);
					}

					left += monitor.ResolutionX;
					right = left + monitor.ResolutionX;
                }
			}

			Process mstsc = new Process();
			mstsc.StartInfo.FileName = "mstsc";
			mstsc.Start();
		}
	}

	public class ResetCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			RegistryUtils.CleanupRegistry();
		}
	}

	internal static class RegistryUtils
	{
		public static void CleanupRegistry()
		{
			using (RegistryKey userKey = Registry.CurrentUser)
			using (RegistryKey terminalServerKey = userKey.OpenSubKey(@"Software\Microsoft\Terminal Server Client", writable: true))
			{
				try
				{
					terminalServerKey.DeleteValue("MonitorConfigSource");
					terminalServerKey.DeleteSubKey(@"Monitors\0");
					terminalServerKey.DeleteSubKey(@"Monitors\1");
					terminalServerKey.DeleteSubKey(@"Monitors\2");
					terminalServerKey.DeleteSubKey(@"Monitors\3");
				}
				// Ignore if the key was not found
				catch (Exception ex) { }
			}
		}
	}
}
