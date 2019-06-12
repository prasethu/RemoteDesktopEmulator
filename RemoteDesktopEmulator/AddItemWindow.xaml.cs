using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RemoteDesktopEmulator
{
    /// <summary>
    /// Interaction logic for AddItemWindow.xaml
    /// </summary>
    internal partial class AddItemWindow : Window
    {
        public static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register("HasError",
                                                                                                 typeof(bool),
                                                                                                 typeof(AddItemWindow),
                                                                                                 new FrameworkPropertyMetadata(false));

        private int _errorCount = 0;

        public AddItemWindow(object dataContext)
        {
            if (dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            InitializeComponent();
            DataContext = dataContext;
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

        public object ReturnValue
        {
            get;
            private set;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            ReturnValue = DataContext;
            DialogResult = true;
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

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled    = true;
                DialogResult = false;
            }
        }
    }
}
