using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace RemoteDesktopEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        public static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register("HasError",
                                                                                                 typeof(bool),
                                                                                                 typeof(MainWindow),
                                                                                                 new FrameworkPropertyMetadata(false));

        private int _errorCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            if (Settings.UpgradeRequired)
            {
                Settings.Upgrade();
                Settings.UpgradeRequired = false;
                Settings.Save();
            }

            DataContext = Serializer.Deserialize(Settings.Data);
        }

        public bool HasError
        {
            get
            {
                return (bool)GetValue(HasErrorProperty);
            }
            set
            {
                SetValue(HasErrorProperty, value);
            }
        }

        private Model Model
        {
            get
            {
                return (Model)DataContext;
            }
        }

        private Properties.Settings Settings
        {
            get
            {
                return Properties.Settings.Default;
            }
        }

        private void AddConfigurationButtonClick(object sender, RoutedEventArgs e)
        {
            Model.AddConfiguration();
        }

        private void AddDisplayButtonClick(object sender, RoutedEventArgs e)
        {
            Model.Configurations[Model.SelectedIndex].AddDisplay();
        }

        private void ConfigurationNameTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            Keyboard.ClearFocus();
        }

        private void ConfigurationNameTextBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox configurationNameTextBox = sender as TextBox;
            if (configurationNameTextBox == null)
                return;

            configurationNameTextBox.IsReadOnly = false;
            configurationNameTextBox.SelectAll();
        }

        private void ConfigurationNameTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox configurationNameTextBox = sender as TextBox;
            if (configurationNameTextBox == null)
                return;

            configurationNameTextBox.IsReadOnly = true;
        }

        private void DisplayComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                return;

            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null)
                return;

            object oldItem = e.RemovedItems[0];

            if (oldItem is Resolution)
                UpdateComboBoxSelection(comboBox, Model.Settings.AvailableResolutions, (Resolution)oldItem);
            else if (oldItem is Scale)
                UpdateComboBoxSelection(comboBox, Model.Settings.AvailableScales, (Scale)oldItem);
            else
                throw new InvalidOperationException();
        }

        private void RemoteConnectButtonClick(object sender, RoutedEventArgs e)
        {
            SaveConfigurationsButtonClick(sender, e);

            try
            {
                RemoteDesktop.Connect(Model.Configurations[Model.SelectedIndex]);
            }
            catch (Exception ex)
            {
                string additionalInfo = string.Empty;
                if (!Elevation.IsProcessElevated)
                    additionalInfo = "\r\n\r\nRunning the application as administrator could resolve the error.";

                MessageBox.Show($"Couldn't start the remote desktop session.\r\n\r\n{ex.Message}{additionalInfo}",
                                "Error connecting...",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void RemoveConfigurationButtonClick(object sender, RoutedEventArgs e)
        {
            Model.RemoveConfiguration();
        }

        private void RemoveDisplayButtonClick(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;
            if (deleteButton == null)
                return;

            Display display = deleteButton.DataContext as Display;
            if (display == null)
                return;

            Model.Configurations[Model.SelectedIndex].RemoveDisplay(display);
        }

        private void SaveConfigurationsButtonClick(object sender, RoutedEventArgs e)
        {
            string data = Serializer.Serialize(Model);
            if (data.Length == 2)
            {
                MessageBox.Show("Couldn't save the current environment.\r\nThe serializer returned an empty string.",
                                "Error saving...",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            Settings.Data = data;
            Settings.Save();
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow window = new SettingsWindow(Model.Settings);
            window.Owner = this;
            window.ShowDialog();
        }

        private void UpdateComboBoxSelection<T>(ComboBox comboBox, IList<T> list, T oldItem)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render,
                                   (Action)(() =>
                                   {
                                       if (list.Contains(oldItem))
                                           comboBox.SelectedItem = oldItem;
                                       else
                                           comboBox.SelectedItem = list[0];
                                   }));
        }

        private void ValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _errorCount++;
            else if (e.Action == ValidationErrorEventAction.Removed)
                _errorCount--;
            else
                throw new NotSupportedException();

            HasError = (_errorCount > 0);
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            SaveConfigurationsButtonClick(sender, new RoutedEventArgs());
            RemoteDesktop.Cleanup();
        }
    }
}
