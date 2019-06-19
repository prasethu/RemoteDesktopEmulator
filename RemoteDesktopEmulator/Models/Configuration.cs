using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RemoteDesktopEmulator
{
    internal class Configuration : ObservableObject
    {
        public const int MaxDisplays = 4;

        private const int MinDisplays = 1;

        private const string DefaultName = "New Config";

        private string _name;

        private Configuration(string name, IList<Display> displays)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            if (displays == null)
                throw new ArgumentNullException(nameof(displays));

            Displays = new ObservableCollection<Display>(displays);
            Name     = name;

            if (Displays.Count == 0)
                AddDisplay(isPrimary: true);
        }

        public bool CanAddDisplay
        {
            get
            {
                return Displays.Count < MaxDisplays;
            }
        }

        public bool CanChangePrimary
        {
            get
            {
                return Displays.Count > MinDisplays;
            }
        }

        public bool CanRemoveDisplay
        {
            get
            {
                return Displays.Count > MinDisplays;
            }
        }

        public ObservableCollection<Display> Displays
        {
            get;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value.Contains(Serializer.Separator.ToString()))
                    throw new NotSupportedException();

                SetValue(ref _name, value);
            }
        }

        public void AddDisplay(bool isPrimary = false)
        {
            if (!CanAddDisplay)
                return;

            if (Displays.Count == 0)
                isPrimary = true;

            Displays.Add(Display.CreateDefault(isPrimary));

            NotifyPropertyChanged(nameof(CanAddDisplay));
            NotifyPropertyChanged(nameof(CanChangePrimary));
            NotifyPropertyChanged(nameof(CanRemoveDisplay));
        }

        public void RemoveDisplay(Display display)
        {
            if (!CanRemoveDisplay)
                return;

            Displays.Remove(display);

            if ((Displays.Count > 0) && (display.IsPrimary))
                Displays[0].IsPrimary = true;

            NotifyPropertyChanged(nameof(CanAddDisplay));
            NotifyPropertyChanged(nameof(CanChangePrimary));
            NotifyPropertyChanged(nameof(CanRemoveDisplay));
        }

        public static Configuration Create(string name, IList<Display> displays)
        {
            return new Configuration(name, displays);
        }

        public static Configuration CreateDefault()
        {
            return Create(DefaultName,
                          new List<Display>());
        }
    }
}
