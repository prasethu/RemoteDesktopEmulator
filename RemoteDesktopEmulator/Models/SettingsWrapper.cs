using System;
using System.Linq;

namespace RemoteDesktopEmulator
{
    internal class SettingsWrapper : ObservableObject
    {
        private const int MaxResolutions = 10;
        private const int MaxScales      = 10;

        private bool _isDirty;

        public SettingsWrapper(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            OriginalSettings = settings;
            Resolutions      = new ListSetting<Resolution>(settings.AvailableResolutions, MaxResolutions);
            Scales           = new ListSetting<Scale>(settings.AvailableScales, MaxScales);
        }

        public bool CanApply
        {
            get
            {
                return CanClose && IsDirty;
            }
        }

        public bool CanClose
        {
            get
            {
                return (Resolutions.Values.Count > 0) &&
                       (Scales.Values.Count > 0);
            }
        }

        private bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                SetValue(ref _isDirty, value);
                NotifyPropertyChanged(nameof(CanApply));
            }
        }

        private Settings OriginalSettings
        {
            get;
        }

        public ListSetting<Resolution> Resolutions
        {
            get;
        }

        public ListSetting<Scale> Scales
        {
            get;
        }

        public void AddResolution(Resolution resolution)
        {
            if (Resolutions.Add(resolution))
                IsDirty = true;
        }

        public void AddScale(Scale scale)
        {
            if (Scales.Add(scale))
                IsDirty = true;
        }

        public void RemoveResolution()
        {
            if (Resolutions.Remove())
                IsDirty = true;
        }

        public void RemoveScale()
        {
            if (Scales.Remove())
                IsDirty = true;
        }

        public void SaveSettings()
        {
            if (IsDirty)
            {
                OriginalSettings.UpdateResolutions(Resolutions.Values.ToList());
                OriginalSettings.UpdateScales(Scales.Values.ToList());

                IsDirty = false;
            }
        }

        public static SettingsWrapper Create(Settings settings)
        {
            return new SettingsWrapper(settings);
        }
    }
}
