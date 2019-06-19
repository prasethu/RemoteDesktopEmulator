using System;

namespace RemoteDesktopEmulator
{
    internal class Display : ObservableObject
    {
        private bool _isPrimary;
        private Resolution _resolution;
        private Scale _scale;

        private Display(bool isPrimary, Resolution resolution, Scale scale)
        {
            if (resolution == null)
                throw new ArgumentNullException(nameof(resolution));

            if (scale == null)
                throw new ArgumentNullException(nameof(scale));

            IsPrimary  = isPrimary;
            Resolution = resolution;
            Scale      = scale;
        }

        public bool IsPrimary
        {
            get
            {
                return _isPrimary;
            }
            set
            {
                SetValue(ref _isPrimary, value);
            }
        }

        public Resolution Resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                SetValue(ref _resolution, value);
            }
        }

        public Scale Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                SetValue(ref _scale, value);
            }
        }

        public static Display Create(bool isPrimary, Resolution resolution, Scale scale)
        {
            return new Display(isPrimary, resolution, scale);
        }

        public static Display CreateDefault(bool isPrimary = false)
        {
            return Create(isPrimary,
                          Resolution.CreateDefault(),
                          Scale.CreateDefault());
        }
    }
}
