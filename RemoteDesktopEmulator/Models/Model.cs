using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RemoteDesktopEmulator
{
    internal class Model : ObservableObject
    {
        private const int MaxConfigurations = 10;
        private const int MinConfigurations = 1;

        private int _selectedIndex = -1;

        private Model(IList<Configuration> configurations, int selectedIndex, Settings settings)
        {
            if (configurations == null)
                throw new ArgumentNullException(nameof(configurations));

            if ((configurations.Count > 0) &&
                ((selectedIndex < 0) || (selectedIndex >= configurations.Count)))
                throw new ArgumentOutOfRangeException(nameof(selectedIndex));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Configurations = new ObservableCollection<Configuration>(configurations);
            SelectedIndex  = selectedIndex;
            Settings       = settings;

            if (Configurations.Count == 0)
                AddConfiguration();
        }

        public bool CanAddConfiguration
        {
            get
            {
                return Configurations.Count < MaxConfigurations;
            }
        }

        public bool CanRemoveConfiguration
        {
            get
            {
                return Configurations.Count > MinConfigurations;
            }
        }

        public ObservableCollection<Configuration> Configurations
        {
            get;
            private set;
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                SetValue(ref _selectedIndex, value);
            }
        }

        public Settings Settings
        {
            get;
            private set;
        }

        public void AddConfiguration()
        {
            if (!CanAddConfiguration)
                return;

            Configurations.Add(Configuration.CreateDefault());
            SelectedIndex = Configurations.Count - 1;

            NotifyPropertyChanged(nameof(CanAddConfiguration));
            NotifyPropertyChanged(nameof(CanRemoveConfiguration));
        }

        public void RemoveConfiguration()
        {
            if (!CanRemoveConfiguration)
                return;

            int index = SelectedIndex;
            Configurations.RemoveAt(index);
            index--;

            if ((index < 0) && (Configurations.Count > 0))
                index = 0;

            SelectedIndex = index;

            NotifyPropertyChanged(nameof(CanAddConfiguration));
            NotifyPropertyChanged(nameof(CanRemoveConfiguration));
        }

        public static Model Create(IList<Configuration> configurations, int selectedIndex, Settings settings)
        {
            return new Model(configurations,
                             selectedIndex,
                             settings);
        }

        public static Model CreateDefault()
        {
            return Create(new List<Configuration>(),
                          0,
                          Settings.CreateDefault());
        }
    }
}
