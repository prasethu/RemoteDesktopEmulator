using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RemoteDesktopEmulator
{
	public class MonitorViewModel : INotifyPropertyChanged
	{
		public MonitorViewModel(int id, int resolutionX, int resolutionY, int dpiScaling)
		{
			_id          = id;
			_resolutionX = resolutionX;
			_resolutionY = resolutionY;
			_dpiScaling  = dpiScaling;
		}

		private int _id;

		public int Id
		{
			get
			{
				return _id;
			}

			set
			{
				if (_id != value)
				{
					_id = value;
					RaisePropertyChanged();
				}
			}
		}

		private int _resolutionX;

		public int ResolutionX
		{
			get
			{
				return _resolutionX;
			}

			set
			{
				if (_resolutionX != value)
				{
					_resolutionX = value;
					RaisePropertyChanged();
				}
			}
		}

		private int _resolutionY;

		public int ResolutionY
		{
			get
			{
				return _resolutionY;
			}

			set
			{
				if (_resolutionY != value)
				{
					_resolutionY = value;
					RaisePropertyChanged();
				}
			}
		}

		private int _dpiScaling;

		public int DpiScaling
		{
			get
			{
				return _dpiScaling;
			}

			set
			{
				if (_dpiScaling != value)
				{
					_dpiScaling = value;
					RaisePropertyChanged();
				}
			}
		}


		private ObservableCollection<Resolution> _commonResolutions = new ObservableCollection<Resolution>()
		{
			new Resolution()
			{
				Value        = "1024x768"
			},
			new Resolution()
			{
				Value        = "1920x1080"
			},
			new Resolution()
			{
				Value = "3840x2160"
			}
		};
		public ObservableCollection<Resolution> CommonResolutions
		{
			get { return _commonResolutions; }
		}

		private ObservableCollection<int> _dpiScalingFactors = new ObservableCollection<int>()
		{
			100,
            125,
			150,
			200,
			250,
			300,
			400
		};
		public ObservableCollection<int> DpiScalingFactors
		{
			get { return _dpiScalingFactors; }
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
	}

	public class Resolution
	{
		public string Value { get; set; }

		public ICommand ClickCommand { get; set; }
	}
}
