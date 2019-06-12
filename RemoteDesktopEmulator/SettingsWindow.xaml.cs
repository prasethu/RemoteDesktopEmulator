using System;
using System.Windows;
using System.Windows.Controls;

namespace RemoteDesktopEmulator
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    internal partial class SettingsWindow : Window
    {
        public SettingsWindow(Settings settings)
        {
            InitializeComponent();
            DataContext = SettingsWrapper.Create(settings);
        }

        private SettingsWrapper SettingsWrapper
        {
            get
            {
                return (SettingsWrapper)DataContext;
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            Button addButton = sender as Button;
            if (addButton == null)
                return;

            object dataContext = addButton.DataContext;
            if (dataContext == null)
                return;

            if (dataContext is ListSetting<Resolution>)
            {
                Resolution newResolution = CreateNewItem(Resolution.CreateDefault());
                if (newResolution == null)
                    return;

                SettingsWrapper.AddResolution(newResolution);
            }
            else if (dataContext is ListSetting<Scale>)
            {
                Scale newScale = CreateNewItem(Scale.CreateDefault());
                if (newScale == null)
                    return;

                SettingsWrapper.AddScale(newScale);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsWrapper.SaveSettings();
        }

        private T CreateNewItem<T>(T defaultValue) where T : class
        {
            AddItemWindow window = new AddItemWindow(defaultValue);
            window.Owner = this;

            if (window.ShowDialog() == true)
                return window.ReturnValue as T;
            else
                return null;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsWrapper.SaveSettings();
            DialogResult = true;
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            Button removeButton = sender as Button;
            if (removeButton == null)
                return;

            object dataContext = removeButton.DataContext;
            if (dataContext == null)
                return;

            if (dataContext is ListSetting<Resolution>)
                SettingsWrapper.RemoveResolution();
            else if (dataContext is ListSetting<Scale>)
                SettingsWrapper.RemoveScale();
            else
                throw new InvalidOperationException();
        }
    }
}
