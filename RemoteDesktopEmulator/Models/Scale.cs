using System;

namespace RemoteDesktopEmulator
{
    internal class Scale : ObservableObject, IComparable<Scale>, IEquatable<Scale>
    {
        private const int DefaultScale = 100;
        private const int MinScale = 100;

        private int _value;

        private Scale(int value)
        {
            if (value < MinScale)
                throw new ArgumentException(nameof(value));

            Value = value;
        }

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value < MinScale)
                    throw new ArgumentOutOfRangeException(nameof(Value));

                SetValue(ref _value, value);
            }
        }

        public int CompareTo(Scale other)
        {
            if (other == null)
                return -1;

            if (Value < other.Value)
                return -1;
            else if (Value > other.Value)
                return 1;

            return 0;
        }

        public bool Equals(Scale other)
        {
            if (other.Value != Value)
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"{Value}%";
        }

        public static Scale Create(int value)
        {
            return new Scale(value);
        }

        public static Scale CreateDefault()
        {
            return Create(DefaultScale);
        }
    }
}
