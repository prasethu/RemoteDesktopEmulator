using System;

namespace RemoteDesktopEmulator
{
    internal class Resolution : ObservableObject, IComparable<Resolution>, IEquatable<Resolution>
    {
        private const int DefaultHeight = 1080;
        private const int DefaultWidth = 1920;
        private const int MinHeight = 600;
        private const int MinWidth = 800;

        private int _height;
        private int _width;

        private Resolution(int width, int height)
        {
            if (height < MinHeight)
                throw new ArgumentException(nameof(height));

            if (width < MinWidth)
                throw new ArgumentException(nameof(width));

            Height = height;
            Width  = width;
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < MinHeight)
                    throw new ArgumentOutOfRangeException(nameof(Height));

                SetValue(ref _height, value);
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value < MinWidth)
                    throw new ArgumentOutOfRangeException(nameof(Width));

                SetValue(ref _width, value);
            }
        }

        public int CompareTo(Resolution other)
        {
            if (other == null)
                return -1;

            if (Width < other.Width)
                return -1;
            else if (Width > other.Width)
                return 1;

            if (Height < other.Height)
                return -1;
            else if (Height > other.Height)
                return 1;

            return 0;
        }

        public bool Equals(Resolution other)
        {
            if (other.Height != Height)
                return false;

            if (other.Width != Width)
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"{Width} x {Height}";
        }

        public static Resolution Create(int width, int height)
        {
            return new Resolution(width, height);
        }

        public static Resolution CreateDefault()
        {
            return Create(DefaultWidth, DefaultHeight);
        }
    }
}
