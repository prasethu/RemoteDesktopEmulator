using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RemoteDesktopEmulator
{
    internal class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (!object.Equals(property, value))
            {
                property = value;
                NotifyPropertyChanged(propertyName);
            }
        }
    }
}
