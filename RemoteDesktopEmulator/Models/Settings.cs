using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RemoteDesktopEmulator
{
    internal class Settings : ObservableObject
    {
        private Settings(IList<Resolution> resolutions, IList<Scale> scales)
        {
            if (resolutions == null)
                throw new ArgumentNullException(nameof(resolutions));

            if (scales == null)
                throw new ArgumentNullException(nameof(scales));

            AvailableResolutions = new ObservableCollection<Resolution>(resolutions);
            AvailableScales      = new ObservableCollection<Scale>(scales);
        }

        public ObservableCollection<Resolution> AvailableResolutions
        {
            get;
            private set;
        }

        public ObservableCollection<Scale> AvailableScales
        {
            get;
            private set;
        }

        public void UpdateResolutions(List<Resolution> newResolutions)
        {
            UpdateList(AvailableResolutions, newResolutions);
        }

        public void UpdateScales(List<Scale> newScales)
        {
            UpdateList(AvailableScales, newScales);
        }

        private void UpdateList<T>(ObservableCollection<T> list, List<T> newEntries)
        {
            list.Clear();
            newEntries.Sort();

            foreach (T entry in newEntries)
                list.Add(entry);
        }

        public static Settings Create(IList<Resolution> resolutions, IList<Scale> scales)
        {
            return new Settings(resolutions, scales);
        }

        public static Settings CreateDefault()
        {
            return Create(new List<Resolution>()
                          {
                              Resolution.Create(1280,  720),
                              Resolution.Create(1920, 1080),
                              Resolution.Create(1920, 1280),
                              Resolution.Create(2160, 1440),
                              Resolution.Create(2560, 1440),
                              Resolution.Create(3000, 2000),
                              Resolution.Create(3240, 2160),
                              Resolution.Create(3840, 2160),
                          },
                          new List<Scale>()
                          {
                              Scale.Create(100),
                              Scale.Create(125),
                              Scale.Create(150),
                              Scale.Create(175),
                              Scale.Create(200),
                              Scale.Create(250),
                              Scale.Create(300),
                              Scale.Create(350),
                          });
        }
    }
}
