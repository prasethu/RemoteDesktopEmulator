using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RemoteDesktopEmulator
{
    internal class ListSetting<T> : ObservableObject
    {
        private int _selectedIndex = 0;

        public ListSetting(IList<T> values, int maxCount)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (maxCount < 1)
                throw new ArgumentException(nameof(maxCount));

            if (values.Count > maxCount)
                throw new ArgumentOutOfRangeException(nameof(values));

            MaxCount      = maxCount;
            SelectedIndex = (values.Count) > 0 ? 0 : -1;
            Values        = new ObservableCollection<T>(values);
        }

        public bool CanAdd
        {
            get
            {
                return Values.Count < MaxCount;
            }
        }

        public bool CanRemove
        {
            get
            {
                return Values.Count > 0;
            }
        }

        public int MaxCount
        {
            get;
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

        public ObservableCollection<T> Values
        {
            get;
        }

        public bool Add(T newValue)
        {
            if (newValue == null)
                throw new ArgumentNullException(nameof(newValue));

            if (!CanAdd)
                return false;

            if (Values.Contains(newValue))
                return false;

            List<T> newValues = Values.ToList();

            newValues.Add(newValue);
            newValues.Sort();

            Values.Clear();

            foreach (T value in newValues)
            {
                Values.Add(value);
            }

            SelectedIndex = Values.IndexOf(newValue);

            NotifyPropertyChanged(nameof(CanAdd));
            NotifyPropertyChanged(nameof(CanRemove));

            return true;
        }

        public bool Remove()
        {
            if (!CanRemove)
                return false;

            if (SelectedIndex == -1)
                return false;

            int index = SelectedIndex;
            Values.RemoveAt(index);
            index--;

            if ((index < 0) && (Values.Count > 0))
                index = 0;

            SelectedIndex = index;

            NotifyPropertyChanged(nameof(CanAdd));
            NotifyPropertyChanged(nameof(CanRemove));

            return true;
        }
    }
}
